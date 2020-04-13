using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyPlayerElement : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI nameText;

    public void Init(string playerName, RectTransform parent)
    {
        nameText.text = playerName;
        rectTransform.SetParent(parent, false);
        rectTransform.SetAsLastSibling();
    }
}
