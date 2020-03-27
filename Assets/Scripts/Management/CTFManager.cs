using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class CTFManager : MonoBehaviour
{
    private static CTFManager instance;

    public static CTFManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<CTFManager>();
            return instance;
        }
    }

    public Action OnTeamScored;

    [Header("Prefabs")]
    public GameObject flagZonePrefab;

    [Header("Settings")]
    public int flagGoal = 3;
    public Timer timer;

    private List<Flag> flags = new List<Flag>();

    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            FlagZoneSpawn[] allFlagZoneSpawns = FindObjectsOfType<FlagZoneSpawn>();
            List<Zone> zones = new List<Zone>();

            for (int i = 0; i < allFlagZoneSpawns.Length; i++)
            {
                Zone zone = Instantiate(flagZonePrefab, allFlagZoneSpawns[i].transform.position, allFlagZoneSpawns[i].transform.rotation, allFlagZoneSpawns[i].transform.parent).GetComponent<Zone>();
                zone.teamIndex = allFlagZoneSpawns[i].teamIndex;
                zone.type = allFlagZoneSpawns[i].type;
                zone.radius = allFlagZoneSpawns[i].radius;
                zones.Add(zone);
            }

            UIManager.Instance.RegisterFlagZones(zones);
        }

        timer.Start();
    }

    private void Update()
    {
        timer.Update();
    }

    public void TeamWins(Team team)
    {
        Debug.Log("Team " + team.index + " wins!");
    }

    public void RegisterFlag(Flag flag)
    {
        flags.Add(flag);
    }
}
