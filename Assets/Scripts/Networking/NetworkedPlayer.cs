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

    public Player player;
    public Character playerCharacter;
    public Rigidbody playerRb;

    public GameObject playerCamera;

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
}
