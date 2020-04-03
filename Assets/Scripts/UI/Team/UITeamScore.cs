using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITeamScore : MonoBehaviour
{
    public int teamIndex = 0;
    public TextMeshProUGUI textScore;
    public Image image;

    private void Start()
    {
        image.color = TeamManager.Instance.GetTeamColor(teamIndex);
        CTFManager.Instance.OnTeamScored += UpdateScore;
    }

    public void UpdateScore()
    {
        int score = TeamManager.Instance.GetScore(teamIndex);
        textScore.text = score.ToString();
    }
}
