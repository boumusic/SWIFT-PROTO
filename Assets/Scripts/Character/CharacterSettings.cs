using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSettings : ScriptableObject
{
    #region Movement

    [Header("Run")]
    public float runSpeed = 5f;
    public float accelerationSpeed = 1f;
    public AnimationCurve accelerationCurve;
    
    public float decelerationSpeed = 1f;
    public float jumpDecelerationSpeed = 1f;
    public AnimationCurve decelerationCurve;

    [Header("Jump")]
    public int jumpCount = 2;
    public float jumpStrength = 5f;
    public float jumpProgressSpeed = 2f;
    public AnimationCurve jumpCurve = new AnimationCurve();
    public bool resetVelocityOnJump = false;

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

    [Header("Slide Against Walls")]
    public bool slideAgainstWalls = true;
    public float slideWallCastLength;
    public AnimationCurve curveWallSlide;
    public float minSlideWallSpeed = 0.15f;
    public float maxSlideWallSpeed = 0.5f;

    [Header("Wall Climb")]
    public float wallClimbSpeed = 1f;

    #endregion

    [Header("Health")]
    public int healthPoints = 1;

    [Header("Attack")]
    public float attackDelay = 0.1f;
    public int damage = 1;
    public float attackWidth = 2f;
    public float attackLength = 2f;
    public float attackHeight = 2f;
    public float attackDuration = 0.7f;
}
