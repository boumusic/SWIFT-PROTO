using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwiftPowerDirection
{
    Input,
    Vector,
    Forward
}

public class SwiftPowerStats : AbilityStats
{
    public float duration = 1f;
    public float speed = 1f;
    public AnimationCurve curve;
    public float gravity = 0f;
    public SwiftPowerDirection directionType;
    public Vector3 direction;
}
