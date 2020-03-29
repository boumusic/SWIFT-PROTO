using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScoreboard : MonoBehaviour
{
    public GameObject playerScorePrefab;
    public Transform[] parents;

    public void RegisterPlayer(Player player)
    {
        //GameObject newScore = Instantiate(playerScorePrefab, parents[player.TeamIndex]);
        //UIPlayerScoreboard ui = newScore.GetComponent<UIPlayerScoreboard>();
        //ui.Initialize(player);
    }
}
