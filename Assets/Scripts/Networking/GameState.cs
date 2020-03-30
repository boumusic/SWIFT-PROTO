using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class GameState : CTFGameStateBehavior
{
    public static GameState Instance;

    private void Awake()
    {
        Instance = this;
    }

    float originalTimer;

    UITeamScore[] UIscores;
    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (!networkObject.IsOwner)
        {
            CTFManager.Instance.timer.TimeLeft = networkObject.timer;
        }
        else
        {
            networkObject.originalTimer = CTFManager.Instance.timer.TimeLeft;
        }
    }

    private void Update()
    {
        if (networkObject.IsOwner)
        {
            networkObject.timer = CTFManager.Instance.timer.TimeLeft;
        }
    }

    public override void AddPoint(RpcArgs args)
    {
        int teamIndex = args.GetAt<int>(0);

        TeamManager.Instance.Score(teamIndex);
        CTFManager.Instance.OnTeamScored?.Invoke();

        if (TeamManager.Instance.GetScore(teamIndex) == CTFManager.Instance.flagGoal)
        {
            string message = "team " + teamIndex + " won !";
            UIManager.Instance.LogMessage(message);

            TeamManager.Instance.teams[0].Score = 0;
            TeamManager.Instance.teams[1].Score = 0;

            CTFManager.Instance.OnTeamScored?.Invoke();

            if (networkObject.IsServer)
            {
                networkObject.timer = networkObject.originalTimer;
            }

            CTFManager.Instance.timer.TimeLeft = networkObject.originalTimer;

            foreach (var player in FindObjectsOfType<NetworkedPlayer>())
            {
                player.networkObject.SendRpc(NetworkedPlayer.RPC_RESPAWN, Receivers.Owner);
                player.networkObject.SendRpc(NetworkedPlayer.RPC_TOGGLE_FLAG, Receivers.All, false);

                player.networkObject.hasFlag = false;

                if (networkObject.IsServer)
                {
                    player.flag = null;
                }
            }
        }

    }
    public override void EndGame(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }
}
