using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnAction", menuName = "Gameplay/PowerActions/SpawnAction", order = 100)]
public class InstantiateAction : PowerAction
{
    public int indexInPool;

    public Vector3 pos;
    public ProjectileStats Projectile = new ProjectileStats();
    public AOEStats AOE = new AOEStats();
}
