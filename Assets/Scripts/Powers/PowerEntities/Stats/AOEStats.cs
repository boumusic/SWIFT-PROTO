using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AOEStats : PowerEntityStats
{
    [Header("AOE Stats")]
    public float radius = 2f;
    public bool kill = false;
    public bool knockback = false;
}
