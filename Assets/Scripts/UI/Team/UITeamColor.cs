using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class UITeamColor : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Instance != null) return;

        Color col = UIManager.Instance.Player.TeamColor;
        Image image = GetComponent<Image>();
        image.color = new Color(col.r, col.g, col.b, image.color.a);
    }

    public void SetColor(Color col)
    {
        Image image = GetComponent<Image>();
        image.color = new Color(col.r, col.g, col.b, image.color.a);
    }
}
