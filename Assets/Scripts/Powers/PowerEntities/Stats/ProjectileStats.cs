using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileStats : PowerEntityStats
{
    [Header("Projectile Stats")]
    [Range(0.001f, 50f)] public float speed = 1;
    [Range(-10f, 10f)] public float gravity = 0f;
    [Range(-10f, 10f)] public float maxFallSpeed = 0f;
}
