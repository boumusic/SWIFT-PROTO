using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSettings : ScriptableObject
{
    #region Movement

    [Header("Run")]
    public float runSpeed = 5f;
    [Range(0.01f, 20)] public float accelerationSpeed = 1f;
    public AnimationCurve accelerationCurve;

    [Range(1, 20)] public float decelerationSpeed = 1f;
    public AnimationCurve decelerationCurve;

    public float dotSpeedMultiplier = 0.9f;
    public float flagMultiplier = 0.9f;

    [Header("Jump")]
    public int jumpCount = 2;
    public float shortJumpStrength = 5f;
    public float jumpStrength = 5f;
    public float jumpChargeSpeed = 5f;
    public float jumpProgressSpeed = 2f;
    public AnimationCurve jumpCurve = new AnimationCurve();
    public bool resetVelocityOnJump = false;
    public float coyoteTime = 0.3f;

    [Range(1, 20)] public float jumpDecelerationSpeed = 1f;
    [Range(0.01f, 20)] public float jumpAccelerationSpeed = 1f;

    [Header("Fall")]
    public float fallStrength = 2f;
    public float fallProgressSpeed = 1f;
    public AnimationCurve fallCurve;

    [Header("Dash")]
    public float dashStrength = 1f;
    public AnimationCurve dashCurve;
    public float dashCooldown = 3f;
    public float dashFlagCooldown = 5f;
    public float dashProgressSpeed = 2f;
    public bool resetDashOnLand = true;
    public bool resetDashOnWallclimb = true;

    [Header("Grounded")]
    public float groundRaycastDown = 1f;
    public float groundRaycastUp = 1f;
    public float groundCastRadius = 0.3f;
    public float castBoxWidth = 0.2f;
    public LayerMask groundMask;

    //[Header("Slide Against Walls")]
    //public bool slideAgainstWalls = true;
    //public AnimationCurve curveWallSlide;
    //public float minSlideWallSpeed = 0.15f;
    //public float maxSlideWallSpeed = 0.5f;

    [Header("Wall Climb")]
    public float wallClimbSpeed = 1f;
    public float wallClimbDuration = 1.8f;
    public AnimationCurve curveWallClimb;
    public float wallCastLength = 0.8f;
    public float ledgeCastMaxHeight = 2.6f;
    public float ledgeCastMinHeight = 1f;
    public float wallClimbConsumeThreshold = 0.01f;

    #endregion

    [Header("Health")]
    public int healthPoints = 1;

    [Header("Attack")]
    public float attackDelay = 0.1f;
    public float attackDuration = 0.7f;
    public float attackCooldown = 0.7f;
    public int damage = 1;
    public float attackWidth = 2f;
    public float attackLength = 2f;
    public float attackHeight = 2f;

    [Header("Attack Impulse")]
    public float impulseStrength = 2f;
    public float impulseFall = 2f;
    public float impulseDuration = 1f;
    public AnimationCurve impulseCurve;

    [Header("Knockback")]
    public float kbStrengthVerti = 10f;
    public float kbStrengthHoriz = 10f;
    public AnimationCurve kbCurveHoriz;
    public float kbProgressSpeed = 1f;
    public AnimationCurve kbCurveVerti;
    public AnimationCurve kbVelocityInfluence;
}
