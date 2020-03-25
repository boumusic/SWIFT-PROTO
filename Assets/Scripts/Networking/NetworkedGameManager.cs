using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class NetworkedGameManager : MonoBehaviour
{
    private void Start()
    {
        ReplaceFlags();

        NetworkManager.Instance.InstantiateNetworkedPlayer();
    }

    void ReplaceFlags()
    {
        if (!NetworkManager.Instance.IsServer) return;

        FlagZoneSpawn[] allFlagZoneSpawns = FindObjectsOfType<FlagZoneSpawn>();

        for (int i = 0; i < allFlagZoneSpawns.Length; i++)
        {
            NetworkedFlagBehavior flag =  NetworkManager.Instance.InstantiateNetworkedFlag(position: allFlagZoneSpawns[i].transform.position, rotation: allFlagZoneSpawns[i].transform.rotation);
            flag.networkObject.teamIndex = allFlagZoneSpawns[i].teamIndex;
        }
    }
}
