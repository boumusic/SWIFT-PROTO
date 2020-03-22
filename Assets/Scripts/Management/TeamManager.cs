﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
    public int index = 0;
    public List<Player> players = new List<Player>();
    public int PlayerCount => players.Count;
    public Color Color { get => color; }
    public int Score { get => score; }

    private int score;
    private Color color;

    public void EarnPoint(int point = 1)
    {
        score += point;
        if (score >= CTFManager.Instance.flagGoal)
        {
            CTFManager.Instance.TeamWins(this);
        }
    }

    public Team(int index, Color color)
    {
        this.index = index;
        this.color = color;
    }

    public void Join(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            Debug.Log(player.PlayerName + " joined team " + index);
        }
    }

    public void Leave(Player player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            Debug.Log(player.PlayerName + " left team " + index);
        }
    }
}

public class TeamManager : MonoBehaviour
{
    private static TeamManager instance;
    public static TeamManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<TeamManager>();
            return instance;
        }
    }

    public static int TeamCount = 2;
    public List<Team> teams = new List<Team>();
    public List<Color> colors = new List<Color>();

    public int GetIndex(Player player)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].players.Contains(player))
            {
                return i;
            }
        }

        return 0;
    }

    private void Awake()
    {
        InitializeTeams();
    }

    private void InitializeTeams()
    {
        for (int i = 0; i < TeamCount; i++)
        {
            teams.Add(new Team(i, colors[i]));
        }
    }

    public void JoinSmallestTeam(Player player)
    {
        SmallestTeam()?.Join(player);
    }

    public void JoinTeam(int i, Player player)
    {
        teams[i].Join(player);
    }

    public int GetScore(int i)
    {
        if (teams.Count > i)
            return teams[i].Score;
        return 0;
    }

    public void Score(int i)
    {
        teams[i].EarnPoint();
    }

    public Color GetTeamColor(int i)
    {
        if (teams.Count > 0)
        {
            return teams[i].Color;
        }

        return Color.white;
    }

    private Team SmallestTeam()
    {
        Team smallest = null;
        for (int i = 0; i < teams.Count; i++)
        {
            if (smallest != null)
            {
                if (teams[i].PlayerCount < smallest.PlayerCount)
                {
                    smallest = teams[i];
                }
            }

            else
            {
                smallest = teams[i];
            }
        }

        return smallest;
    }
}

public interface ITeamAffilitation
{
    void UpdateAffiliation();
}