using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamColor : MonoBehaviour
{
    private void Start()
    {
        Color col = UIManager.Instance.Player.TeamColor;
        Image image = GetComponent<Image>();
        image.color = new Color(col.r, col.g, col.b, image.color.a);
    }
}
