using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatMaker : MonoBehaviour
{
	private void Awake()
	{
		if (NetworkManager.Instance.IsServer) SceneManager.sceneLoaded += CreateInlineChat;
	}

	private void CreateInlineChat(Scene arg0, LoadSceneMode arg1)
	{
		SceneManager.sceneLoaded -= CreateInlineChat;
		var chat = NetworkManager.Instance.InstantiateChatManager();
		DontDestroyOnLoad(chat.gameObject);
	}
}