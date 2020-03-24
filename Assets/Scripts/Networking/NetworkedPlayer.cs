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
    public GameObject tpsCharacter;
    public CharacterAnimator characterAnimator;
    public TextMeshPro nameText;

    public string playerName = "DefaultName";

    // variables

    public int teamIndex;
    public Flag flag;
    public bool HasFlag => flag != null;

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

            nameText.GetComponentInParent<LookAtCamera>().cam = Camera.main.transform;

            tpsCharacter.SetActive(true);

            networkObject.Networker.disconnected += x =>
            {
                networkObject.Destroy();
            };
        }
        else
        {
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
            characterAnimator.onDeathAnim += () =>
            {
                networkObject.SendRpc(RPC_DIE, Receivers.Others);
            };

        }
    }


    private Vector3 previousPosition;
    private void Update()
    {
        if (!networkObject.IsOwner)
        {
            //rotation
            tpsCharacter.transform.rotation = networkObject.rotation;

            // position
            characterTransform.position = networkObject.position;

            // run animation
            Vector3 velocity = networkObject.localVelocity;

            Vector3 runVelocity = velocity;
            runVelocity.y = 0;
            bool isRunning = runVelocity.magnitude > 0.005f;

            characterAnimator.Run(isRunning, velocity);

            previousPosition = characterTransform.position;

            // climb animation
            characterAnimator.WallClimb(networkObject.climbing);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                player.enabled = false;
            }
            if (Input.GetMouseButtonDown(0) &&
            Application.isFocused)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                player.enabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (networkObject.IsOwner)
        {
            networkObject.position = characterTransform.position;
            networkObject.rotation = tpsCharacter.transform.rotation;
            networkObject.localVelocity = playerCharacter.Velocity;
            networkObject.climbing = playerCharacter.CurrentState == CharacterState.WallClimbing;
        }
    }

    void SetName()
    {
        playerName = PlayerInfoManager.Instance.playerName;
        nameText.text = playerName;

        nameText.enabled = false;

        networkObject.SendRpc(RPC_CHANGE_NAME, Receivers.AllBuffered, PlayerInfoManager.Instance.playerName);
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
        Debug.Log(networkObject.IsOwner ? "owner " : "remote " + "is jumping");
        characterAnimator.Jump();
    }

    public override void Land(RpcArgs args)
    {
        characterAnimator.Land();
    }

    public override void Die(RpcArgs args)
    {
        characterAnimator.Death();
    }

    public override void Destroy(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }
}
