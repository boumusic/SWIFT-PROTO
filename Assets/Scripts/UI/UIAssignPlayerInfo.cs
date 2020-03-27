using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public enum PlayerInfo
{
    Name
}

public class UIAssignPlayerInfo : MonoBehaviour
{
    public PlayerInfo toAssign;
    private TextMeshProUGUI text;

    private void OnDrawGizmosSelected()
    {
        GetText();
    }

    private void GetText()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (text == null)GetText();
        switch (toAssign)
        {
            case PlayerInfo.Name:
                text.text = NetworkManager.Instance == null ? UIManager.Instance.Player.PlayerName : PlayerInfoManager.Instance.playerName;
                break;
        }
    }
}
