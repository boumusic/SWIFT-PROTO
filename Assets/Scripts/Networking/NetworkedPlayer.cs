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

    private string playerName = "DefaultName";

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
        }
        else
        {
            SetName();

            playerCharacter.OnAttack += () =>
            {
                networkObject.SendRpc(RPC_ATTACK, Receivers.All);
            };

            networkObject.Networker.disconnected += x =>
            {
                networkObject.SendRpc(RPC_DESTROY, Receivers.All);
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
            Vector3 velocity = characterTransform.position - previousPosition;
            bool isRunning = velocity.magnitude > .05f;
            characterAnimator.Run(isRunning, tpsCharacter.transform.InverseTransformDirection(velocity.normalized));
            previousPosition = characterTransform.position;

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                player.enabled = false;
                player.Character.playerCamera.InputMouse(Vector2.zero);
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
    }

    public override void Destroy(RpcArgs args)
    {
        Destroy(this.gameObject);
    }

    public override void ChangeName(RpcArgs args)
    {
        playerName = args.GetAt<string>(0);
        nameText.text = playerName;
    }
}
