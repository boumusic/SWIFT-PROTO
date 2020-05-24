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
        if (!NetworkManager.Instance.IsServer) return;

        NetworkedPlayer player = other.transform.root.GetComponent<NetworkedPlayer>();
        if (player != null)
        {
            if (!player.isAlive) return;
            Debug.Log(player.playerName + " died in zone " + gameObject.name);


            player.networkObject.alive = false;

            player.networkObject.SendRpc(NetworkedPlayerBehavior.RPC_DIE, Receivers.All, "VOID", 0);

            // return flag

            if (player.flag == null) return;

            player.flag.GetComponentInParent<Zone>().networkObject.SendRpc(Zone.RPC_RETRIEVED, Receivers.All, "VOID", player.playerName);

            player.flag = null;
            player.networkObject.hasFlag = false;

            player.networkObject.SendRpc(NetworkedPlayer.RPC_TOGGLE_FLAG, true, Receivers.AllBuffered, false);
        }
    }
}
