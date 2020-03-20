using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterMovement : ScriptableObject
{
    [Header("Run")]
    public float runSpeed = 5f;
    public float acceleration = 0.5f;

    [Header("Jump")]
    public int jumpCount = 2;
    public float jumpStrength = 5f;
    public float jumpProgressSpeed = 2f;
    public AnimationCurve jumpCurve = new AnimationCurve();

    [Header("Fall")]
    public float fallStrength = 2f;
    public float fallProgressSpeed = 1f;
    public AnimationCurve fallCurve;

    [Header("Grounded")]
    public float groundRaycastDown = 1f;
    public float groundRaycastUp = 1f;
    public float groundCastRadius = 0.3f;
    public float castBoxWidth = 0.2f;
    public LayerMask groundMask;

    [Header("Desired Direction")]
    public float desiredCastLength;
    public float minSlideWallSpeed = 0.15f;
    public float maxSlideWallSpeed = 0.5f;
}
