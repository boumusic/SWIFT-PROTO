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
    public int minutes = 10;

    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            FlagZoneSpawn[] allFlagZoneSpawns = FindObjectsOfType<FlagZoneSpawn>();

            for (int i = 0; i < allFlagZoneSpawns.Length; i++)
            {
                Zone zone = Instantiate(flagZonePrefab, allFlagZoneSpawns[i].transform.position, allFlagZoneSpawns[i].transform.rotation, allFlagZoneSpawns[i].transform.parent).GetComponent<Zone>();
                zone.teamIndex = allFlagZoneSpawns[i].teamIndex;
            }
        }
    }

    public void TeamWins(Team team)
    {
        Debug.Log("Team " + team.index + " wins!");
    }

}
