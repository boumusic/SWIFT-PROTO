using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using MonsterLove.StateMachine;

public class NetworkedPlayer : NetworkedPlayerBehavior
{
    public Transform characterTransform;

    [Header("To destroy if not owner")]
    public Player player;
    public Character playerCharacter;
    public GameObject playerCamera;

    [Header("References")]
    public Rigidbody playerRb;
    public Animator armsAnimator;

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
            
            networkObject.Networker.disconnected += x =>
            {
                Destroy(this.gameObject);
            };
        }
        else
        {
            playerCharacter.OnAttack += () =>
            {
                networkObject.SendRpc(RPC_ATTACK, Receivers.All);
            };
        }
    }


    private void Update()
    {
        if (!networkObject.IsOwner)
        {
            characterTransform.position = networkObject.position;
        }
    }

    private void FixedUpdate()
    {
        if (networkObject.IsOwner)
        {
            networkObject.position = characterTransform.position;
        }
    }

    public override void Attack(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            testBall.transform.position = characterTransform.position;
        });
    }
}
