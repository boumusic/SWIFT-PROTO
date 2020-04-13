using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ServerElement : MonoBehaviour
{
    public RectTransform rectTransform;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI adressText;
    public TextMeshProUGUI playerCountText;

    public void Init(string name, string adress, int playerCount, RectTransform parent)
    {
        this.nameText.text = name;
        this.adressText.text = adress;
        this.playerCountText.text = playerCount == -1 ? "?/8" : playerCount + "/8";

        rectTransform.SetParent(parent, false);
    }
}
