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

    public bool isAlive;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        teamIndex = networkObject.teamIndex;
        UpdateTeamColor();

        if (!networkObject.IsOwner)
        {
            playerRb.isKinematic = true;

            Destroy(player);
            Destroy(playerCharacter.GetComponent<StateMachineRunner>());
            Destroy(playerCharacter);
            Destroy(playerCamera);

            nameText.GetComponentInParent<LookAtCamera>().cam = Camera.main.transform;

            tpsCharacter.SetActive(true);
        }
        else
        {
            UIManager.Instance.AssignPlayer(this.player);

            networkObject.UpdateInterval = (ulong)16.6667f;

            SetName();

            characterAnimator.onLandAnim += () =>
            {
                networkObject.SendRpc(RPC_LAND, Receivers.Others);
            };
            characterAnimator.onJumpAnim += () =>
            {
                networkObject.SendRpc(RPC_JUMP, Receivers.Others);
            };
            characterAnimator.onAttackAnim += () =>
            {
                networkObject.SendRpc(RPC_ATTACK, Receivers.Others);
            };

        }
        
        if (networkObject.IsServer)
        {
            isAlive = true;
            networkObject.alive = isAlive;
        }
    }

    private void Update()
    {
        if (!networkObject.IsOwner)
        {
            //rotation
            tpsCharacter.transform.rotation = networkObject.rotation;

            // position
            characterTransform.position = networkObject.position;

            // run animation
            characterAnimator.Run(networkObject.running, networkObject.localVelocity);

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
            characterTransform.position = Vector3.zero;
        }
        else
        {
            coll.enabled = true;

            //todo
        }

        if (NetworkManager.Instance.IsServer)
        {
            isAlive = true;
            networkObject.alive = isAlive;
        }

        characterAnimator.Land();
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
        characterAnimator.Jump();
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
        if (!isAlive) return;

        isAlive = false;

        networkObject.SendRpc(RPC_DIE, Receivers.All, args.GetNext<string>(), args.GetNext<int>());
    }
}
