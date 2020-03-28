using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFlag : UI360
{
    [Header("UI Flag")]
    public FlagZoneType type;
    public int teamIndex;
    public Image[] images;

    public TextMeshProUGUI text;

    private bool SameTeam => teamIndex == UIManager.Instance.Player.TeamIndex;
    private bool Local => NetworkedGameManager.Instance == null;

    public override void Update()
    {
        base.Update();
        UpdateText();
    }

    private void UpdateText()
    {
        string newText = "";
        switch (type)
        {
            case FlagZoneType.Altar:
                if (!SameTeam)
                {
                    if (Local)
                    {
                        if (!UIManager.Instance.Player.Character.HasFlag)
                        {
                            newText = "CAPTURE";
                        }
                    }

                    else
                    {
                        if (!UIManager.Instance.NetworkedPlayer.HasFlag)
                        {
                            newText = "CAPTURE";
                        }
                    }
                }

                else
                {
                    if (Local)
                    {
                        if (!UIManager.Instance.Player.Character.HasFlag)
                        {
                            newText = "REACH";
                        }
                    }

                    else
                    {
                        if (!UIManager.Instance.NetworkedPlayer.HasFlag)
                        {
                            newText = "REACH";
                        }
                    }

                    
                }
                break;

            case FlagZoneType.Shrine:
                if (!SameTeam)
                {
                    newText = "";
                }

                else
                {
                    if (Local)
                    {
                        if (UIManager.Instance.Player.Character.HasFlag)
                        {
                            newText = "REACH";
                        }
                    }

                    else
                    {
                        if (UIManager.Instance.NetworkedPlayer.HasFlag)
                        {
                            newText = "REACH";
                        }
                    }

                }
                break;
        }

        text.text = newText;

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

        for (int i = 0; i < images.Length; i++)
        {
            Color col = TeamManager.Instance.GetTeamColor(teamIndex);
            images[i].color = new Color(col.r, col.g, col.b, images[i].color.a);
        }
    }
}
