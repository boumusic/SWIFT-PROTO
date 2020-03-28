using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public enum FlagZoneType
{
    Altar,
    Shrine
}

public class Zone : NetworkedFlagBehavior, ITeamAffilitation
{
    public int teamIndex = 0;
    public FlagZoneType type;
    public float radius;

    private bool isAltar => type == FlagZoneType.Altar;

    [Header("Components")]
    public SphereCollider coll;
    public Flag flag;
    public Renderer rend;
    public ParticleSystem capturedFx;
    public ParticleSystem scoredFx;

    public bool IsCaptured => !flag.gameObject.activeInHierarchy;
    
    private void OnDrawGizmos()
    {
        if (teamIndex == 0)
        {
            Gizmos.color = new Color(1, 0.23f, 0.37f, 1f);
        }

        else
        {
            Gizmos.color = new Color(0.14f, 0.65f, 1f, 1f);
        }

        Gizmos.DrawWireSphere(transform.position, radius);
        UpdateRadius();
    }

    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            UpdateAffiliation();
            UpdateRadius();
        }
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        teamIndex = networkObject.teamIndex;

        UpdateAffiliation();
    }

    public void UpdateAffiliation()
    {
        Color col = TeamManager.Instance.GetTeamColor(teamIndex);
        Color curr = rend.material.GetColor("_Color");
        rend.material.SetColor("_Color", new Color(col.r, col.g, col.b, curr.a));
        flag.SetTeamIndex(teamIndex);

        if (type == FlagZoneType.Shrine) flag.gameObject.SetActive(false);
    }

    private void UpdateRadius()
    {
        coll.radius = radius;
        rend.gameObject.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
    }

    private void OnTriggerStay(Collider other)
    {
        // NETWORKING

        if (NetworkManager.Instance != null)
        {
            if (!NetworkManager.Instance.IsServer) return;

            NetworkedPlayer player = other.transform.root.GetComponent<NetworkedPlayer>();
            if (player != null)
            {
                if (!IsCaptured)
                {
                    if ((player.networkObject.teamIndex != networkObject.teamIndex) && isAltar)
                    {
                        networkObject.SendRpc(RPC_CAPTURED, Receivers.All, player.playerName);
                        networkObject.isFlagThere = false;

                        player.flag = flag;
                        player.networkObject.hasFlag = true;
                    }
                }

                if (player.HasFlag && player.teamIndex == teamIndex && isAltar)
                {
                    player.flag = null;
                    player.networkObject.hasFlag = false;

                    networkObject.SendRpc(RPC_SCORED, Receivers.All, player.playerName, player.teamIndex);

                    for (int i = 0; i < NetworkedGameManager.Instance.flagZones.Count; i++)
                    {
                        if (NetworkedGameManager.Instance.flagZones[i] == this) continue;

                        NetworkedGameManager.Instance.flagZones[i].networkObject.SendRpc(RPC_RESPAWN_FLAG, Receivers.All);
                    }
                }
            }

            return;
        }
        // NETWORKING

        Character chara;
        if (other.gameObject.TryGetComponent(out chara))
        {
            if (!IsCaptured)
            {
                if (chara.TeamIndex != teamIndex && isAltar)
                {
                    chara.Capture(flag);
                    capturedFx.Play();
                }
            }

            if (chara.HasFlag && chara.TeamIndex == teamIndex && !isAltar)
            {
                chara.Score();
                scoredFx.Play();
            }
        }
    }

    public override void Retrieved(RpcArgs args)
    {
        string message = args.GetAt<string>(0) + " retrieved the flag from " + args.GetAt<string>(1) + "!";
        UIManager.Instance.LogMessage(message);

        flag.gameObject.SetActive(true);
    }

    public override void Scored(RpcArgs args)
    {
        string message = args.GetAt<string>(0) + " scored for team " + args.GetAt<int>(1) + "!";
        UIManager.Instance.LogMessage(message);

        TeamManager.Instance.Score(args.GetAt<int>(1));
        CTFManager.Instance.OnTeamScored?.Invoke();

        scoredFx.Play();
    }

    public override void Captured(RpcArgs args)
    {
        UIManager.Instance.LogMessage(args.GetAt<string>(0) + " captured the Flag !");
        flag.gameObject.SetActive(false);
        capturedFx.Play();
    }

    public override void RespawnFlag(RpcArgs args)
    {
        flag.gameObject.SetActive(true);
    }
}
