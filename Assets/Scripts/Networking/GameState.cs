using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class GameState : CTFGameStateBehavior
{
    public override void EndGame(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    UITeamScore[] UIscores;
    protected override void NetworkStart()
    {
        base.NetworkStart();

        UIscores = FindObjectsOfType<UITeamScore>();

    }
}
