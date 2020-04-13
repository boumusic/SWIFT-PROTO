using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class UIFlag : MonoBehaviour
{
    [Header("UI Ping")]
    public FlagZoneType type;
    public int teamIndex;
    public Image[] images;

    public TextMeshProUGUI text;

    private bool SameTeam => Local ? teamIndex == UIManager.Instance.Player.TeamIndex : teamIndex == UIManager.Instance.NetworkedPlayer.networkObject.teamIndex;
    private bool Local => NetworkedGameManager.Instance == null;

    public void Update()
    {
        if (!Local && UIManager.Instance.Player == null) return;

        //base.Update();
        UpdateText();
    }

    private bool PlayerHasFlag => Local ? UIManager.Instance.Player.Character.HasFlag : UIManager.Instance.NetworkedPlayer.networkObject.hasFlag;

    private void UpdateText()
    {
        string newText = "";
        bool important = false;
       
        switch (type)
        {
            case FlagZoneType.Altar:
                if (!SameTeam)
                {
                    if(!PlayerHasFlag)
                            newText = "CAPTURE";
                }

                else
                {
                    if(!PlayerHasFlag)
                        newText = "DEFEND";
                }
                break;

            case FlagZoneType.Shrine:
                if (!SameTeam)
                {
                    newText = "";
                }

                else
                {
                    if(PlayerHasFlag)
                    {
                        newText = "REACH";
                        important = true;
                    }
                }
                break;
        }

        text.text = newText;
        UpdateVisuals(important);
        bool toggle = newText == "";
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(!toggle);
        }

    }

    public void Init(int index, FlagZoneType t)
    {
        type = t;
        teamIndex = index;

        UpdateVisuals(false);
    }

    private void UpdateVisuals(bool important)
    {
        for (int i = 0; i < images.Length; i++)
        {
            Color col = TeamManager.Instance.GetTeamColor(teamIndex);
            float a = important ? 1 : 0.5f;
            images[i].color = new Color(col.r, col.g, col.b, a);
            float scale = important ? 0.45f : 0.3f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
