using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Power", menuName = "Gameplay/Power", order = 100)]
public class PowerSettings : ScriptableObject
{
    [Header("General")]
    public float cooldown = 10f;

    [Header("Actions")]
    public List<PowerActionSettings> actions = new List<PowerActionSettings>();
}