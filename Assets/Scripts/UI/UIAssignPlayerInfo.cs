using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
                text.text = UIManager.Instance.Player.PlayerName;
                break;
        }
    }
}
