using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speed Boost", menuName = "Gameplay/SpeedBoost", order = 100)]
public class SpeedBoostSettings : ScriptableObject
{
    public float duration = 1f;
    public float speedMultiplier = 2f;
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });
    public float flowGiven = 200;
}
