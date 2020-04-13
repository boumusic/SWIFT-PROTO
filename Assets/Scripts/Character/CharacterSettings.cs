using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSettings : ScriptableObject
{
    #region Movement

    [Header("Run")]
    public float runSpeed = 5f;
    [Range(0, 20)] public float accelerationTime = 0.05f;
    public AnimationCurve accelerationCurve;

    [Range(0, 20)] public float decelerationAmount = 0.5f;
    public AnimationCurve decelerationCurve;
    
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

    [Range(0f, 20)] public float jumpDecelerationAmount = 1f;
    [Range(0f, 20)] public float jumpAccelerationTime = 1f;

    [Header("Fall")]
    public float fallStrength = 2f;
    public float fallProgressSpeed = 1f;
    public AnimationCurve fallCurve;
    public float wallSlideSpeedMul = 0.5f;

    [Header("Dash")]
    public Propulsion dash;
    public float dashCooldown = 3f;
    public float dashFlagCooldown = 5f;

    [Header("Grounded")]
    public float groundRaycastDown = 1f;
    public float groundRaycastUp = 1f;
    public float groundCastRadius = 0.3f;
    public float castBoxWidth = 0.2f;
    public LayerMask groundMask;
    
    [Header("Wall Climb")]
    public float wallClimbSpeed = 1f;
    public float wallClimbDuration = 1.8f;
    public AnimationCurve curveWallClimb;
    public float wallCastLength = 0.8f;
    public float ledgeCastMaxHeight = 2.6f;
    public float ledgeCastMinHeight = 1f;
    public float wallClimbConsumeThreshold = 0.01f;

    [Header("Walljump")]
    public Propulsion wallJump;

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
    public float attackSizeY = 2f;
    public Vector3 attackCenter;

    [Header("Attack Impulse")]
    public Propulsion attackImpulse;

    [Header("Knockback")]
    public Propulsion knockback;
}
