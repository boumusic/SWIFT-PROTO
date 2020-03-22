using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour, ITeamAffilitation
{
    public int teamIndex = 0;
    public Flag flag;
    public Renderer rend;

    public bool IsCaptured => !flag.gameObject.activeInHierarchy;

    private void Start()
    {
        UpdateAffiliation();
    }

    public void UpdateAffiliation()
    {
        Color col = TeamManager.Instance.GetTeamColor(teamIndex);
        Color curr = rend.material.GetColor("_Color");
        rend.material.SetColor("_Color", new Color(col.r, col.g, col.b, curr.a));
        flag.SetTeamIndex(teamIndex);
    }

    private void OnTriggerStay(Collider other)
    {
        Character chara;
        if(other.gameObject.TryGetComponent(out chara))
        {
            if(!IsCaptured)
            {
                if(chara.TeamIndex != teamIndex)
                {
                    chara.Capture(flag);
                }                
            }

            if(chara.HasFlag && chara.TeamIndex == teamIndex)
            {
                chara.Score();
            }            
        }
    }
}
