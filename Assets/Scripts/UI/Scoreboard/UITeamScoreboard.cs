using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UITeamScoreboard : MonoBehaviour
{
    [Header("Position")]
    public float spacing = 20f;
    public float offset = 20f;
    private List<UIPlayerScoreboard> playerScoreboards = new List<UIPlayerScoreboard>();

    /*
    public void RegisterPlayerScoreboard(UIPlayerScoreboard playerScoreboard)
    {
        playerScoreboards.Add(playerScoreboard);
        Order();
    }

    public void Order()
    {
        playerScoreboards = playerScoreboards.OrderBy(playerScoreboard => playerScoreboard.Score.rank) as List<UIPlayerScoreboard>;

        for (int i = 0; i < playerScoreboards.Count; i++)
        {
            Vector3 curr = playerScoreboards[i].transform.localPosition;
            float y = i * spacing + offset;
            playerScoreboards[i].transform.localPosition = new Vector3(curr.x, y, curr.z);
        }
    }
    */
}
