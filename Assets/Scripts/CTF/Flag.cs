using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private Character owner;
    public bool IsCaptured => owner != null;
    public Renderer[] rends;
    private int teamIndex = 0;

    private void Start()
    {
        CTFManager.Instance.RegisterFlag(this);
    }

    public void SetTeamIndex(int index)
    {
        this.teamIndex = index;
        UpdateAffiliation();
    }

    public void UpdateAffiliation()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material.SetColor("_Color", TeamManager.Instance.GetTeamColor(teamIndex));
        }
    }

    public void CapturedBy(Character owner)
    {
        this.owner = owner;
    }
}
