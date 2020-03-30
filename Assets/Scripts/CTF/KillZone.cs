using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Character chara;
        if (other.TryGetComponent(out chara))
        {
            Debug.Log(chara.PlayerName + " died in zone " + gameObject.name);
            chara.NPlayer.networkObject.SendRpc(NetworkedPlayerBehavior.RPC_DIE, Receivers.Server);
            chara.Die();
        }
    }
}
