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

    [Header("Flow")]
    public float flowStateThreshold = 100f;
    public float maxFlow = 200;
    public float flowCamYawDiff = 0.8f;
    public float flowLostPerSecondsWallSlide = 35;
    public float flowLostPerSecondsStrafe = 40;
    public float flowLostPerSecondsOnStop = 50;
    public float flowLostOnParry = 100;
    public float flowLostOnAttack = 50;

    [Header("Flow Run")]
    [Tooltip("How many flow per second earned when running forward.")] public float flowPerSecondRun = 25f;
    public float flowSpeedMul = 1.1f;
    public float flowHighJumpAirDragMax = 1.05f;
    public float flowShortJumpAirDragMax = 1.1f;
    public float earnFlowRunSpeedThreshold = 0.9f;

    [Tooltip("How long after the beginning of a fall the character can earn flow.")] public float flowFallDelay = 1f;
    [Tooltip("Flow earned per wall Jump")] public float flowPerWallJump = 20f;

    [Header("Flow Fall")]
    [Tooltip("How many flow per second earned when falling.")] public float flowPerSecondFall = 50f;
    public float flowFallHeightThreshold = 5;
    public float flowFallJumpBuffer = 0.2f;
    public float flowFallDistanceMultiplier = 5f;

    [Header("Jump")]
    public int jumpCount = 2;
    public float jumpStrength = 5f;
    public float jumpChargeSpeed = 5f;
    public float jumpProgressSpeed = 2f;
    public AnimationCurve jumpCurve = new AnimationCurve();
    public bool resetVelocityOnJump = false;
    public float jumpAirDrag = 0.9f;
    public float coyoteTime = 0.3f;

    [Header("Short Jump")]
    public float shortJumpStrength = 5f;

    [Range(0f, 20)] public float jumpDecelerationAmount = 1f;
    [Range(0f, 20)] public float jumpAccelerationTime = 1f;

    [Header("Fall")]
    public float fallStrength = 2f;
    public float fallProgressSpeed = 1f;
    public AnimationCurve fallCurve;

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
    public float wallClimbHorizSpeedMultiplier = 0.3f;
    public float wallClimbSpeed = 1f;
    public float wallClimbDuration = 1.8f;
    public AnimationCurve curveWallClimb;
    public float wallCastLength = 0.8f;
    public float wallCastUpHeight = 0.8f;
    public float ledgeCastMaxHeight = 2.6f;
    public float ledgeCastMinHeight = 1f;
    public float wallClimbConsumeThreshold = 0.01f;

    [Header("WallSlide")]
    public float wallSlideSpeed = 10f;
    public float wallSlideProgressSpeed = 3f;
    public AnimationCurve wallSlideCurve;

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
