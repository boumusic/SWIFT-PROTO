using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    [Header("Settings")]
    public int flagGoal = 3;
    public int minutes = 10;    

    public void TeamWins(Team team)
    {
        Debug.Log("Team " + team.index + " wins!");
    }

}
