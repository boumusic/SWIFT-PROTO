using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using MonsterLove.StateMachine;
using TMPro;

public class NetworkedPlayer : NetworkedPlayerBehavior
{
    public bool owner = false;

    public Transform characterTransform;

    [Header("To destroy if not owner")]
    public Player player;
    public Character playerCharacter;
    public GameObject playerCamera;

    [Header("References")]
    public Rigidbody playerRb;
    public Collider coll;
    public GameObject tpsCharacter;
    public CharacterAnimator characterAnimator;
    public TextMeshPro nameText;

    public string playerName = "DefaultName";

    // variables

    public int teamIndex;
    public Flag flag;
    public bool HasFlag => flag != null;

    public bool isAlive = true;

    private void Start()
    {
        //playerCamera.SetActive(false);
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (!networkObject.IsOwner)
        {
            playerRb.isKinematic = true;

            Destroy(player);
            Destroy(playerCharacter.GetComponent<StateMachineRunner>());
            Destroy(playerCharacter);
            Destroy(playerCamera);

            tpsCharacter.SetActive(true);

            foreach (var lookat in FindObjectsOfType<LookAtCamera>())
            {
                lookat.cam = Camera.main.transform;
            }

            Init(networkObject.teamIndex, networkObject.position);

        }
        else
        {
            networkObject.teamIndex = PlayerInfoManager.Instance.playerTeam;

            Debug.Log("flag zones count : " + NetworkedGameManager.Instance.flagZones.Count);

            Vector3 spawnPos = NetworkedGameManager.Instance.flagZones.Find(f =>
                (f.networkObject.teamIndex == networkObject.teamIndex) && (f.networkObject.type == (int)FlagZoneType.Shrine)
            ).transform.position + Vector3.up;

            networkObject.spawnPos = spawnPos;

            networkObject.SendRpc(RPC_INIT, Receivers.AllBuffered, networkObject.teamIndex, networkObject.spawnPos);

            playerCamera.SetActive(true);

            foreach (var lookat in FindObjectsOfType<LookAtCamera>())
            {
                lookat.cam = playerCamera.transform;
            }

            UIManager.Instance.AssignPlayer(this.player);

            networkObject.UpdateInterval = (ulong)16.6667f;

            SetName();

            characterAnimator.onLandAnim += () =>
            {
                networkObject.SendRpc(RPC_LAND, Receivers.Others);
            };
            characterAnimator.onJumpAnim += () =>
            {
                networkObject.SendRpc(RPC_JUMP, Receivers.Others, false);
            };
            characterAnimator.onDoubleJumpAnim += () =>
            {
                networkObject.SendRpc(RPC_JUMP, Receivers.Others, true);
            };
            characterAnimator.onAttackAnim += () =>
            {
                networkObject.SendRpc(RPC_ATTACK, Receivers.Others);
            };
            characterAnimator.onDashAnim += () =>
            {
                networkObject.SendRpc(RPC_DASH, Receivers.Others);
            };
        }
    }

    private void OnDestroy()
    {
        if (networkObject.IsOwner)
        {
            networkObject.Destroy();
        }
    }

    private void Update()
    {
        owner = networkObject.IsOwner;

        if (!networkObject.IsOwner)
        {
            //rotation
            tpsCharacter.transform.rotation = networkObject.rotation;

            // position
            characterTransform.position = networkObject.position;

            // run animation
            characterAnimator.Run(networkObject.running);
            characterAnimator.Velocity(networkObject.localVelocity.normalized);

            // climb animation
            characterAnimator.WallClimb(networkObject.climbing);
        }
        else
        {
            networkObject.position = characterTransform.position;
            networkObject.rotation = tpsCharacter.transform.rotation;
            networkObject.localVelocity = playerCharacter.Velocity;
            networkObject.climbing = playerCharacter.CurrentState == CharacterState.WallClimbing;
            networkObject.running = playerCharacter.Axis.magnitude != 0;
            networkObject.attacking = playerCharacter.isStartingAttack || playerCharacter.IsAttacking;
            networkObject.viewDir = playerCamera.transform.forward;

            //DebugParry();
        }
    }

    void DebugParry()
    {
        NetworkedPlayer attackingPlayer = null;
        foreach (var player in FindObjectsOfType<NetworkedPlayer>())
        {
            if (player.networkObject.NetworkId == networkObject.NetworkId) continue;

            attackingPlayer = player;
            break;
        }

        if (attackingPlayer != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                playerCharacter.TryAttack();
                attackingPlayer.networkObject.SendRpc(NetworkedPlayer.RPC_DEBUG_ATTACK, Receivers.Owner);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("No matching player found");
            }
        }
    }

    void UpdateTeamColor()
    {
        GetComponentInChildren<SkinnedMeshRenderer>(true).material.SetColor("_Color", TeamManager.Instance.GetTeamColor(teamIndex));

    }

    void SetName()
    {
        playerName = PlayerInfoManager.Instance.playerName;
        nameText.text = playerName;

        nameText.enabled = false;

        networkObject.SendRpc(RPC_CHANGE_NAME, Receivers.AllBuffered, PlayerInfoManager.Instance.playerName);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);

        if (networkObject.IsOwner)
        {
            if (playerCharacter.tps.activeInHierarchy) playerCharacter.ToggleTPS();

            playerCharacter.enabled = true;
            player.enabled = true;
            playerRb.useGravity = false;

            characterTransform.position = networkObject.spawnPos;
            networkObject.position = networkObject.spawnPos;
        }
        else
        {
            coll.enabled = true;
        }

        characterAnimator.Land();

        yield return new WaitForSeconds(1);

        if (NetworkManager.Instance.IsServer)
        {
            isAlive = true;
            networkObject.alive = isAlive;
        }

    }

    public override void Attack(RpcArgs args)
    {
        characterAnimator.Attack();
    }

    public override void ChangeName(RpcArgs args)
    {
        playerName = args.GetAt<string>(0);
        nameText.text = playerName;
    }

    public override void Jump(RpcArgs args)
    {
        characterAnimator.Jump(args.GetAt<bool>(0));
    }

    public override void Land(RpcArgs args)
    {
        characterAnimator.Land();
    }

    public override void Die(RpcArgs args)
    {

        if (networkObject.IsOwner)
        {
            if (!playerCharacter.tps.activeInHierarchy) playerCharacter.ToggleTPS();
            playerCharacter.enabled = false;
            player.enabled = false;
            playerRb.useGravity = true;
        }
        else
        {
            coll.enabled = false;

            //todo : flag
        }

        characterAnimator.Death();

        UIManager.Instance.DisplayKillFeed(args.GetNext<string>(), args.GetNext<int>(), playerName, teamIndex);

        StartCoroutine(Respawn());
    }

    public override void TryHit(RpcArgs args)
    {
        if (!isAlive)
        {
            Debug.Log("client is dead");
            return;
        }

        uint killerID = args.GetAt<uint>(0);
        string killerName = args.GetAt<string>(1);
        int killerTeam = args.GetAt<int>(2);
        Vector3 killerViewDir = args.GetAt<Vector3>(3);

        //check for attacking and viewDir

        NetworkedPlayer attackingPlayer = null;
        foreach (var player in FindObjectsOfType<NetworkedPlayer>())
        {
            if (player.networkObject.NetworkId != killerID) continue;

            attackingPlayer = player;
            break;
        }

        if (attackingPlayer == null)
        {
            Debug.Log("No matching player found");
            return;
        }

        if (attackingPlayer.isAlive == false) return;

        if (networkObject.attacking && Vector3.Dot(attackingPlayer.networkObject.viewDir, networkObject.viewDir) < 0)
        {
            Vector3 direction = attackingPlayer.characterTransform.position - characterTransform.position;

            attackingPlayer.networkObject.SendRpc(NetworkedPlayer.RPC_KNOCKBACK, Receivers.Owner, direction);
            networkObject.SendRpc(NetworkedPlayer.RPC_KNOCKBACK, Receivers.Owner, -direction);

            return;
        }

        // Death

        isAlive = false;
        networkObject.alive = false;

        networkObject.SendRpc(RPC_DIE, Receivers.All, killerName, killerTeam);
        attackingPlayer.networkObject.SendRpc(RPC_HITMARKER, Receivers.Owner);

        // return flag

        if (flag == null) return;

        flag.GetComponentInParent<Zone>().networkObject.SendRpc(Zone.RPC_RETRIEVED, Receivers.All, killerName, playerName);

        flag = null;
        networkObject.hasFlag = false;

        networkObject.SendRpc(RPC_TOGGLE_FLAG, true, Receivers.AllBuffered, false);
    }

    public override void Init(RpcArgs args)
    {
        Init(args.GetAt<int>(0), args.GetAt<Vector3>(1));
    }

    public void Init(int _teamIndex, Vector3 _spawnPos)
    {
        teamIndex = _teamIndex;
        UpdateTeamColor();

        characterTransform.position = _spawnPos;

        if (networkObject.IsOwner)
        {
            networkObject.position = characterTransform.position;

            foreach (var uiTeamColor in FindObjectsOfType<UITeamColor>())
            {
                uiTeamColor.SetColor(TeamManager.Instance.GetTeamColor(teamIndex));
            }
        }

        for (int i = 0; i < flagVisualsRend.Length; i++)
        {
            flagVisualsRend[i].material.SetColor("_Color", TeamManager.Instance.GetOppositeTeamColor(teamIndex));
        }
    }
    
    public GameObject[] flagVisuals;
    public Renderer[] flagVisualsRend;
    public override void ToggleFlag(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            playerCharacter.ToggleFlagVisuals(args.GetAt<bool>(0));
            return;
        }

        for (int i = 0; i < flagVisuals.Length; i++)
        {
            flagVisuals[i].SetActive(args.GetAt<bool>(0));
        }
    }

    public override void Knockback(RpcArgs args)
    {
        playerCharacter.Knockbacked(args.GetNext<Vector3>());
        playerCharacter.CancelAttack();
    }

    public override void DebugAttack(RpcArgs args)
    {
        playerCharacter.TryAttack();
    }

    public override void Hitmarker(RpcArgs args)
    {
        UIManager.Instance.HitMarker();
    }

    public override void Respawn(RpcArgs args)
    {
        characterTransform.position = networkObject.spawnPos + new Vector3(Random.Range(-2f,2f), 0, Random.Range(-2f, 2f));
    }

    public override void Dash(RpcArgs args)
    {
        characterAnimator.Dash();
    }
}
