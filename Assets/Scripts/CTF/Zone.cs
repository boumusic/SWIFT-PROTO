﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;


public class Zone : NetworkedFlagBehavior, ITeamAffilitation
{
    public int teamIndex = 0;

    [Header("Components")]
    public Flag flag;
    public Renderer rend;
    public ParticleSystem capturedFx;
    public ParticleSystem scoredFx;

    public bool IsCaptured => !flag.gameObject.activeInHierarchy;

    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            UpdateAffiliation();
        }
    }

    protected override void NetworkStart()
    {
        teamIndex = networkObject.teamIndex;

        UpdateAffiliation();
    }

    public void UpdateAffiliation()
    {
        Color col = TeamManager.Instance.GetTeamColor(teamIndex);
        Color curr = rend.material.GetColor("_Color");
        rend.material.SetColor("_Color", new Color(col.r, col.g, col.b, curr.a));
        flag.SetTeamIndex(teamIndex);
    }

    private void OnTriggerStay(Collider other)
    {
        // NETWORKING

        if (NetworkManager.Instance != null)
        {
            if (!NetworkManager.Instance.IsServer) return;

            NetworkedPlayer player = other.gameObject.GetComponentInParent<NetworkedPlayer>();
            if (player != null)
            {
                if (!IsCaptured)
                {
                    if (player.teamIndex != teamIndex)
                    {
                        player.flag = flag;
                        networkObject.SendRpc(RPC_STOLEN, Receivers.All, player.playerName);
                        networkObject.isFlagThere = false;
                    }
                }

                if (player.HasFlag && player.teamIndex == teamIndex)
                {
                    player.flag = null;

                    networkObject.SendRpc(RPC_SCORED, Receivers.All, player.playerName, player.teamIndex);
                }
            }

            return;
        }
        // NETWORKING

        Character chara;
        if(other.gameObject.TryGetComponent(out chara))
        {
            if(!IsCaptured)
            {
                if(chara.TeamIndex != teamIndex)
                {
                    chara.Capture(flag);
                    capturedFx.Play();
                }                
            }

            if(chara.HasFlag && chara.TeamIndex == teamIndex)
            {
                chara.Score();
                scoredFx.Play();
            }            
        }
    }


    public override void Retrieved(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override void Scored(RpcArgs args)
    {
        string message = args.GetAt<string>(0) + " scored for team " + args.GetAt<int>(1) + "!";
        UIManager.Instance.LogMessage(message);
        flag.gameObject.SetActive(true);
        TeamManager.Instance.Score(args.GetAt<int>(1));

        scoredFx.Play();
    }

    public override void Stolen(RpcArgs args)
    {
        UIManager.Instance.LogMessage(args.GetAt<string>(0) + " captured the Flag !");
        flag.gameObject.SetActive(false);
        capturedFx.Play();
    }
}
