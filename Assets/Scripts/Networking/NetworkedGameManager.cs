using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using System;

public class NetworkedGameManager : MonoBehaviour
{
    public List<List<NetworkedPlayerNetworkObject>> teams = new List<List<NetworkedPlayerNetworkObject>>();

    public static NetworkedGameManager Instance;

    public List<Zone> flagZones = new List<Zone>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < 2; i++)
        {
            teams.Add(new List<NetworkedPlayerNetworkObject>());
        }
    }

    private void Start()
    {

        if (NetworkManager.Instance.IsServer)
        {
            ReplaceFlags();

            NetworkManager.Instance.playerLoadedScene += OnPlayerJoin;

            // server doesn't have a networkingplayer
            CreatePlayer(null);
        }
    }

    private void OnPlayerJoin(NetworkingPlayer player, NetWorker sender)
    {
        // if teams are full
        if (teams[0].Count == 4 && teams[1].Count == 4)
        {
            NetworkCameraNetworkObject camObject = NetworkManager.Instance.InstantiateNetworkCamera().networkObject;
            camObject.AssignOwnership(player);
            return;
        }

        CreatePlayer(player);
    }

    void CreatePlayer(NetworkingPlayer player)
    {
        NetworkedPlayerNetworkObject playerObject = NetworkManager.Instance.InstantiateNetworkedPlayer().networkObject;

        if (player != null)
        {
            playerObject.AssignOwnership(player);
            player.disconnected += OnPlayerQuit;
        }

        playerObject.teamIndex = GetTeamIndex();
    }

    int GetTeamIndex()
    {
        return teams[0].Count > teams[1].Count ? 1 : 0;
    }

    void OnPlayerQuit(NetWorker sender)
    {
        sender.IterateNetworkObjects(x =>
        {
            x.Destroy();
        });
    }

    void ReplaceFlags()
    {
        FlagZoneSpawn[] allFlagZoneSpawns = FindObjectsOfType<FlagZoneSpawn>();

        for (int i = 0; i < allFlagZoneSpawns.Length; i++)
        {
            NetworkedFlagBehavior flag =  NetworkManager.Instance.InstantiateNetworkedFlag(position: allFlagZoneSpawns[i].transform.position, rotation: allFlagZoneSpawns[i].transform.rotation);
            flag.networkObject.teamIndex = allFlagZoneSpawns[i].teamIndex;
            flag.networkObject.type = (int)allFlagZoneSpawns[i].type;
            flag.networkObject.radius = allFlagZoneSpawns[i].radius;

            flagZones.Add(flag.GetComponent<Zone>());
        }
    }
}
