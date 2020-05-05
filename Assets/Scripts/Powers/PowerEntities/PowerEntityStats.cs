using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerEntityStats
{
    [Header("Lifetime")]
    public float lifetime = 1;

    [Header("Events")]
    public PowerAction onCollideAlly;
    public PowerAction onCollideEnemy;
    public PowerAction onCollideGeo;
    public PowerAction onDeath;
}
