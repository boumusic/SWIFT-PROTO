using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileStats : AbilityStats
{
    [Range(0.001f, 50f)] public float speed = 1;
    [Range(-10f, 10f)] public float gravity = 0f;
    [Range(0f, 10f)] public float lifetime = 3f;
}
