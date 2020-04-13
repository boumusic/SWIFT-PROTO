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

            NetworkManager.Instance.objectInitialized += (x,y) =>
            {
                if (y is NetworkedPlayerNetworkObject)
                {
                    NetworkedPlayerNetworkObject obj = (NetworkedPlayerNetworkObject)y;

                    obj.AuthorityUpdateMode = true;

                    //int teamIndex = GetTeamIndex();
                    //obj.teamIndex = teamIndex;

                    //teams[teamIndex].Add(obj);

                    //obj.spawnPos = spawnPos;
                    //obj.SendRpc(NetworkedPlayerBehavior.RPC_INIT, Receivers.AllBuffered, teamIndex, spawnPos);

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

        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(1);

        if (PlayerInfoManager.Instance.playerTeam == 2 || PlayerInfoManager.Instance.playerTeam == -1)
        {
            NetworkManager.Instance.InstantiateNetworkCamera();
        }
        else
        {
            CreatePlayer();
        }
    }

    void CreatePlayer()
    {
        NetworkManager.Instance.InstantiateNetworkedPlayer();
    }

    int GetTeamIndex()
    {
        return teams[0].Count > teams[1].Count ? 1 : 0;
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
        }
    }
}
