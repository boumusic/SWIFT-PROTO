using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class NetworkedGameManager : MonoBehaviour
{
    public static NetworkedGameManager Instance;

    public List<Zone> flagZones = new List<Zone>();

    private void Awake()
    {
        Instance = this;
    }

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
            flag.networkObject.type = (int)allFlagZoneSpawns[i].type;

            flagZones.Add(flag.GetComponent<Zone>());
        }
    }
}
