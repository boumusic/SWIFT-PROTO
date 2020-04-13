using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using BeardedManStudios.Forge.Networking.Frame;

public class MultiplayerMenu : MonoBehaviour
{
    public RectTransform serversContainer;
    public GameObject serverElementPrefab;
	public InputField ipAddress = null;
	public InputField portNumber = null;
    public InputField playerName;
	public bool DontChangeSceneOnConnect = false;
	public string masterServerHost = string.Empty;
	public ushort masterServerPort = 15940;
    public string natServerHost = string.Empty;
	public ushort natServerPort = 15941;
	public bool connectUsingMatchmaking = false;
	public bool useElo = false;
	public int myElo = 0;
	public int eloRequired = 0;

	public GameObject networkManager = null;
	public GameObject[] ToggledButtons;
	private NetworkManager mgr = null;
	private NetWorker server;

	private List<Button> _uiButtons = new List<Button>();
	private bool _matchmaking = false;
	public bool useMainThreadManagerForRPCs = true;
	public bool useInlineChat = false;

	public bool getLocalNetworkConnections = false;

	public bool useTCP = false;
    bool lan = false;

	private void Start()
	{
        portNumber.text = "16000";

        for (int i = 0; i < ToggledButtons.Length; ++i)
		{
			Button btn = ToggledButtons[i].GetComponent<Button>();
			if (btn != null)
				_uiButtons.Add(btn);
		}

		if (!useTCP)
		{
			// Do any firewall opening requests on the operating system
			NetWorker.PingForFirewall(ushort.Parse(portNumber.text));
		}

		if (useMainThreadManagerForRPCs)
        {
            Rpc.MainThreadRunner = MainThreadManager.Instance;
        }

		if (getLocalNetworkConnections)
		{
			NetWorker.localServerLocated += LocalServerLocated;
		}


        Refresh();
    }

    void ClearServers()
    {
        for (int i = 0; i < serversContainer.childCount; i++)
        {
            Destroy(serversContainer.GetChild(i).gameObject);
        }
    }

    void AddServer(string name, string adress, int playerCount)
    {
        ServerElement serverElement = Instantiate(serverElementPrefab).GetComponent<ServerElement>();

        serverElement.Init(name, adress, playerCount, serversContainer);

        Button serverButton = serverElement.GetComponent<Button>();

        serverButton.onClick.AddListener(() =>
        {
            ipAddress.text = adress;
            Connect();
        });
    }

    public void Online()
    {
        lan = false;
        Refresh();
    }

    public void Lan()
    {
        lan = true;
        Refresh();
    }

    public void Refresh()
    {
        ClearServers();

        if (lan)
        {
            NetWorker.RefreshLocalUdpListings(ushort.Parse(portNumber.text));
            return;
        }

        // The Master Server communicates over TCP
        TCPMasterClient client = new TCPMasterClient();

        // Once this client has been accepted by the master server it should sent it's get request
        client.serverAccepted += x =>
        {
            try
            {
                // The overall game id to select from
                string gameId = "myGame";

                // The game type to choose from, if "any" then all types will be returned
                string gameType = "any";

                // The game mode to choose from, if "all" then all game modes will be returned
                string gameMode = "all";

                // Create the get request with the desired filters
                JSONNode sendData = JSONNode.Parse("{}");
                JSONClass getData = new JSONClass();

                // The id of the game to get
                getData.Add("id", gameId);
                getData.Add("type", gameType);
                getData.Add("mode", gameMode);

                sendData.Add("get", getData);

                // Send the request to the server
                client.Send(BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
            }
            catch
            {
                // If anything fails, then this client needs to be disconnected
                client.Disconnect(true);
                client = null;
            }
        };

        // An event that is raised when the server responds with hosts
        client.textMessageReceived += (player, frame, sender) =>
        {
            try
            {
                // Get the list of hosts to iterate through from the frame payload
                JSONNode data = JSONNode.Parse(frame.ToString());
                if (data["hosts"] != null)
                {
                    // Create a C# object for the response from the master server
                    MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);

                    if (response != null && response.serverResponse.Count > 0)
                    {
                        // Go through all of the available hosts and add them to the server browser
                        foreach (MasterServerResponse.Server server in response.serverResponse)
                        {
                            Debug.Log("Found server " + server.Name);

                            // Update UI

                            MainThreadManager.Run(() =>
                            {
                                AddServer(server.Name, server.Address, server.PlayerCount);
                            });
                        }
                    }
                }
            }
            finally
            {
                if (client != null)
                {
                    // If we succeed or fail the client needs to disconnect from the Master Server
                    client.Disconnect(true);
                    client = null;
                }
            }
        };

        client.Connect(masterServerHost, (ushort)masterServerPort);
    }


    private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
        MainThreadManager.Run(() =>
        {
            AddServer("Local Game", endpoint.Address, -1);
        });
    }

    bool connecting = false;

    public void Connect()
	{
        if (connecting) return;

        connecting = true;

        if (connectUsingMatchmaking)
		{
			ConnectToMatchmaking();
			return;
		}
		ushort port;
		if(!ushort.TryParse(portNumber.text, out port))
		{
			Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
		    	return;
		}

		NetWorker client;

		if (useTCP)
		{
			client = new TCPClient();
			((TCPClient)client).Connect(ipAddress.text, (ushort)port);
		}
		else
		{
			client = new UDPClient();
			if (natServerHost.Trim().Length == 0)
				((UDPClient)client).Connect(ipAddress.text, (ushort)port);
			else
				((UDPClient)client).Connect(ipAddress.text, (ushort)port, natServerHost, natServerPort);
		}

        Connected(client);
	}

	public void ConnectToMatchmaking()
	{
		if (_matchmaking)
			return;

		SetToggledButtons(false);
		_matchmaking = true;

		if (mgr == null && networkManager == null)
			throw new System.Exception("A network manager was not provided, this is required for the tons of fancy stuff");
		
		mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

		mgr.MatchmakingServersFromMasterServer(masterServerHost, masterServerPort, myElo, (response) =>
		{
			_matchmaking = false;
			SetToggledButtons(true);
			Debug.LogFormat("Matching Server(s) count[{0}]", response.serverResponse.Count);

			//TODO: YOUR OWN MATCHMAKING EXTRA LOGIC HERE!
			// I just make it randomly pick a server... you can do whatever you please!
			if (response != null && response.serverResponse.Count > 0)
			{
				MasterServerResponse.Server server = response.serverResponse[Random.Range(0, response.serverResponse.Count)];
				//TCPClient client = new TCPClient();
				UDPClient client = new UDPClient();
				client.Connect(server.Address, server.Port);
				Connected(client);
			}
		});
	}

    public void LANHost()
    {
        masterServerHost = string.Empty;
        ipAddress.text = "localhost";

        Host();
    }

	public void Host()
	{
		if (useTCP)
		{
			server = new TCPServer(64);
			((TCPServer)server).Connect();
		}
		else
		{
			server = new UDPServer(64);

			if (natServerHost.Trim().Length == 0)
				((UDPServer)server).Connect(ipAddress.text, ushort.Parse(portNumber.text));
			else
				((UDPServer)server).Connect(port: ushort.Parse(portNumber.text), natHost: natServerHost, natPort: natServerPort);
		}

		server.playerTimeout += (player, sender) =>
		{
			Debug.Log("Player " + player.NetworkId + " timed out");
		};
		//LobbyService.Instance.Initialize(server);

		Connected(server);
	}

	private void Update()
	{
        /*
		if (Input.GetKeyDown(KeyCode.H))
			Host();
		else if (Input.GetKeyDown(KeyCode.C))
			Connect();
		else if (Input.GetKeyDown(KeyCode.L))
		{
			NetWorker.localServerLocated -= TestLocalServerFind;
			NetWorker.localServerLocated += TestLocalServerFind;
			NetWorker.RefreshLocalUdpListings();
		}*/
	}



	private void TestLocalServerFind(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		Debug.Log("Address: " + endpoint.Address + ", Port: " + endpoint.Port + ", Server? " + endpoint.IsServer);
	}

	public void Connected(NetWorker networker)
	{
		if (!networker.IsBound)
		{
			Debug.LogError("NetWorker failed to bind");
			return;
		}

		if (mgr == null && networkManager == null)
		{
			Debug.LogWarning("A network manager was not provided, generating a new one instead");
			networkManager = new GameObject("Network Manager");
			mgr = networkManager.AddComponent<NetworkManager>();
		}
		else if (mgr == null)
			mgr = Instantiate(networkManager).GetComponent<NetworkManager>();

		// If we are using the master server we need to get the registration data
		JSONNode masterServerData = null;
		if (!string.IsNullOrEmpty(masterServerHost))
		{
			string serverId = "myGame";
            string serverName = PlayerInfoManager.Instance.playerName;
			string type = "Deathmatch";
			string mode = "Teams";
			string comment = "Demo comment...";

			masterServerData = mgr.MasterServerRegisterData(networker, serverId, serverName, type, mode, comment, useElo, eloRequired);
		}

        if (lan)
        {
            mgr.Initialize(networker, string.Empty, masterServerPort, masterServerData);
        }
        else
        {
            mgr.Initialize(networker, masterServerHost, masterServerPort, masterServerData);
        }

		if (useInlineChat && networker.IsServer)
			SceneManager.sceneLoaded += CreateInlineChat;

		if (networker is IServer)
		{
			if (!DontChangeSceneOnConnect)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
			else
            {
                NetworkObject.Flush(networker); //Called because we are already in the correct scene!
            }
		}
	}

	private void CreateInlineChat(Scene arg0, LoadSceneMode arg1)
	{
		SceneManager.sceneLoaded -= CreateInlineChat;
		var chat = NetworkManager.Instance.InstantiateChatManager();
		DontDestroyOnLoad(chat.gameObject);
	}

	private void SetToggledButtons(bool value)
	{
		for (int i = 0; i < _uiButtons.Count; ++i)
			_uiButtons[i].interactable = value;
	}

    public void ChangePlayerName()
    {
        PlayerInfoManager.Instance.playerName = playerName.text;
    }

	private void OnApplicationQuit()
	{
		if (getLocalNetworkConnections)
			NetWorker.EndSession();

		if (server != null) server.Disconnect(true);
	}
}
