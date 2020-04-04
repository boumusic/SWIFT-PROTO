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

            ((UDPServer)NetworkManager.Instance.Networker).playerAccepted += OnPlayerJoin;

            NetworkManager.Instance.objectInitialized += (x,y) =>
            {
                if (y is NetworkedPlayerNetworkObject)
                {
                    NetworkedPlayerNetworkObject obj = (NetworkedPlayerNetworkObject)y;

                    obj.AuthorityUpdateMode = true;

                    int teamIndex = GetTeamIndex();
                    obj.teamIndex = teamIndex;

                    teams[teamIndex].Add(obj);

                    Vector3 spawnPos = flagZones.Find(f => 
                        (f.networkObject.teamIndex == teamIndex) && (f.networkObject.type == (int)FlagZoneType.Shrine)
                    ).transform.position + Vector3.up;

                    obj.spawnPos = spawnPos;
                    obj.SendRpc(NetworkedPlayerBehavior.RPC_INIT, Receivers.AllBuffered, teamIndex, spawnPos);

                    NetworkedPlayerBehavior behavior = obj.AttachedBehavior as NetworkedPlayerBehavior;
                    behavior.GetComponent<NetworkedPlayer>().Init(teamIndex, spawnPos);
                    
                    Debug.Log("red team is " + teams[0].Count + " players/ blue team is " + teams[1].Count + " players");
                }
            };


            //Handle disconnection
            NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
            {
                MainThreadManager.Run(() =>
                {
                    //Loop through all players and find the player who disconnected, store all it's networkobjects to a list
                    List<NetworkObject> toDelete = new List<NetworkObject>();
                    foreach (var no in sender.NetworkObjectList)
                    {
                        if (no.Owner == player)
                        {
                            //Found him
                            toDelete.Add(no);
                        }
                    }

                    //Remove the actual network object outside of the foreach loop, as we would modify the collection at runtime elsewise. (could also use a return, too late)
                    if (toDelete.Count > 0)
                    {
                        for (int i = toDelete.Count - 1; i >= 0; i--)
                        {
                            sender.NetworkObjectList.Remove(toDelete[i]);
                            toDelete[i].Destroy();
                        }
                    }
                });
            };
        }

        if (PlayerInfoManager.Instance.playerName == "spectator")
        {
            NetworkManager.Instance.InstantiateNetworkCamera();
        }
        else
        {
            CreatePlayer();
        }
    }

    private void OnPlayerJoin(NetworkingPlayer player, NetWorker sender)
    {
        MainThreadManager.Run(() =>
        {
            player.disconnected += x =>
            {
                OnPlayerQuit(player);
            };
        });
    }

    void CreatePlayer()
    {
        NetworkManager.Instance.InstantiateNetworkedPlayer();
    }

    int GetTeamIndex()
    {
        return teams[0].Count > teams[1].Count ? 1 : 0;
    }

    void OnPlayerQuit(NetworkingPlayer player)
    {
        return;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < teams[i].Count; j++)
            {
                if (teams[i][j].NetworkId == player.NetworkId)
                {
                    teams[i][j].Destroy();
                    teams[i].RemoveAt(j);
                }
            }
        }
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
