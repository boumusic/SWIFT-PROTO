using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Gameplay", order = 100)]
public class Ability : ScriptableObject
{
    public float cooldown = 10f;
}
