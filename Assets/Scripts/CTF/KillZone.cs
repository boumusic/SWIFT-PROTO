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
            NetworkedPlayer p = chara.NPlayer;
            if (!p) return;

            if (!p.isAlive) return;
            Debug.Log(chara.PlayerName + " died in zone " + gameObject.name);

            
            p.networkObject.alive = false;

            p.networkObject.SendRpc(NetworkedPlayerBehavior.RPC_DIE, Receivers.All, "VOID", 0);

            // return flag

            if (p.flag == null) return;

            p.flag.GetComponentInParent<Zone>().networkObject.SendRpc(Zone.RPC_RETRIEVED, Receivers.All, "VOID", p.playerName);

            p.flag = null;
            p.networkObject.hasFlag = false;

            p.networkObject.SendRpc(NetworkedPlayer.RPC_TOGGLE_FLAG, true, Receivers.AllBuffered, false);
        }
    }
}
