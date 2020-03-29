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
    public PlayerScore Score => player.score;

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
    public void Initialize(Player player)
    {
        this.player = player;
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
