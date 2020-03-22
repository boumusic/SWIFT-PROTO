using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour, ITeamAffilitation
{
    public int teamIndex = 0;
    public Flag flag;
    public Renderer rend;

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

    private void OnTriggerEnter(Collider other)
    {
        Character chara;
        if(other.gameObject.TryGetComponent(out chara))
        {
            if(flag != null)
            {
                Debug.Log(chara.PlayerName + " captured the flag!");
                chara.Capture(flag);
                flag = null;
            }

            else if(chara.HasFlag)
            {
                this.flag = chara.Flag;
                //SCORE FLAG;
                //Feedback etc...
            }
            
        }
    }
}
