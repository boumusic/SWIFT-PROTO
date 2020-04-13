using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum PlayerScoreboardTextType
{
    Rank,
    Name,
    Kills,
    Deaths,
    Captured,
    Scored,
    Retreived
}

[System.Serializable]
public class PlayerScoreboardText
{
    [HideInInspector] public string name;
    public PlayerScoreboardTextType type;
    public TextMeshProUGUI text;
}

public class UIPlayerScoreboard : MonoBehaviour
{
    [Header("Texts")]
    public PlayerScoreboardText[] texts;
    private Player player;
    //public PlayerScore Score => player.score;

    /*
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            string name = texts[i].type.ToString();
            if(texts[i].name != name)
            {
                texts[i].name = name;
            }
        }
    }
    */

    private void Update()
    {
        UpdateScore();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }
    //PlayerScore S => player.score;

    public void UpdateScore()
    {
        //UpdateText(PlayerScoreboardTextType.Name, player.PlayerName);
        //UpdateText(PlayerScoreboardTextType.Kills, S.kills.ToString());
        //UpdateText(PlayerScoreboardTextType.Deaths, S.deaths.ToString());
        //UpdateText(PlayerScoreboardTextType.Captured, S.captured.ToString());
        //UpdateText(PlayerScoreboardTextType.Scored, S.scored.ToString());
        //UpdateText(PlayerScoreboardTextType.Retreived, S.retreived.ToString());
    }

    public void UpdateText(PlayerScoreboardTextType type, string newText)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if(texts[i].type == type)
            {
                texts[i].text.text = newText;
            }
        }
    }
}
