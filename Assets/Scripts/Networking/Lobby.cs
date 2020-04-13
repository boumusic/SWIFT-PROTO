using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby : LobbyBehavior
{
    public GameObject playerElementPrefab;

    public List<RectTransform> teamParents = new List<RectTransform>();

    public List<Button> teamButtons = new List<Button>();

    public Button launchButton;

    public List<LobbyPlayer> players = new List<LobbyPlayer>();

    public int myId;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        networkObject.SendRpc(RPC_CREATE_PLAYER, Receivers.AllBuffered, PlayerInfoManager.Instance.playerName, 2);
        PlayerInfoManager.Instance.ChangeTeam(2);

        if (networkObject.IsServer)
        {
            launchButton.gameObject.SetActive(true);
            launchButton.onClick.AddListener(Launch);
        }

        for (int i = 0; i < teamButtons.Count; i++)
        {
            int index = i;
            teamButtons[i].onClick.AddListener(() =>
            {
                JoinTeam(index);
            });
        }
    }

    void JoinTeam(int teamIndex)
    {
        networkObject.SendRpc(RPC_SWITCH_PLAYER, Receivers.AllBuffered, PlayerInfoManager.Instance.playerName, teamIndex);

        PlayerInfoManager.Instance.ChangeTeam(teamIndex);
    }

    void Launch()
    {
        launchButton.onClick.RemoveAllListeners();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public override void CreatePlayer(RpcArgs args)
    {
        string playerName = args.GetNext<string>();
        int playerTeam = args.GetNext<int>();

        LobbyPlayerElement playerElement = Instantiate(playerElementPrefab).GetComponent<LobbyPlayerElement>();

        playerElement.Init(playerName, teamParents[playerTeam]);

        players.Add(new LobbyPlayer(playerElement, playerName, playerTeam));
    }

    public override void SwitchPlayer(RpcArgs args)
    {
        string playerName = args.GetNext<string>();
        int playerTeam = args.GetNext<int>();

        players[players.FindIndex(x => x.playerName == playerName)].ChangeTeam(playerTeam, teamParents[playerTeam]);
    }
}

public class LobbyPlayer
{
    public LobbyPlayerElement lobbyPlayerElement;
    public string playerName;
    public int playerTeam;

    public LobbyPlayer(LobbyPlayerElement lobbyPlayerElement, string playerName, int playerTeam)
    {
        this.lobbyPlayerElement = lobbyPlayerElement;
        this.playerName = playerName;
        this.playerTeam = playerTeam;
    }

    public void ChangeTeam(int teamIndex, RectTransform parent)
    {
        this.playerTeam = teamIndex;
        lobbyPlayerElement.Init(playerName, parent);
    }
}
