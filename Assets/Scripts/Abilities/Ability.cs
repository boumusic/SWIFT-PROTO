using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAction
{

}

public class SpawnAction : AbilityAction
{
    public GameObject toSpawn;
    public Vector3 position;
}

[CreateAssetMenu(fileName = "Ability", menuName = "Gameplay/Ability", order = 100)]
public class Ability : ScriptableObject
{
    public float cooldown = 10f;
    public List<AbilityAction> actions = new List<AbilityAction>();
}
