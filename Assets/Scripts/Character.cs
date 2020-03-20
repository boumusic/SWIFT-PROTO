using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody body;
    public CharacterMovement m;
    public PlayerCamera playerCamera;

    private Vector2 lastAxis;
    private Vector2 axis;
    private Vector3 DesiredVelocity => CamRotation * new Vector3(axis.x, 0, axis.y);
    private Vector3 velocity;
    private Vector3 currentVel;
    private bool isRunning;
    private float yVelocity;

    private float jumpProgress = 0f;
    private float fallProgress = 0f;
    private int jumpLeft;
    private RaycastHit hit;
    private RaycastHit[] hits = new RaycastHit[5];

    private StateMachine<CharacterState> stateMachine;
    public CharacterState CurrentState => stateMachine.State;
    public Vector3 Velocity => new Vector3(velocity.x, yVelocity, velocity.z);

    public Vector3 Forward => new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z);
    public Vector3 Right => new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z);

    public Vector3 FinalVelocity => CamRotation * WallSlideRotation * Velocity;
    public float FacingVelocity => Mathf.Clamp01(Vector3.Dot(Forward, FinalVelocity));
    public float CameraRoll => Vector3.Dot(Right, FinalVelocity.normalized) * (CurrentState == CharacterState.Jumping ? 3.5f : 1f);

    public Quaternion CamRotation => playerCamera.Forward;

    public bool IsRunning { get => isRunning; }
    public bool IsinAir => CurrentState == CharacterState.Jumping || CurrentState == CharacterState.Falling;

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.groundRaycastUp;
    public Vector3 FeetDestination => FeetOrigin - Vector3.up * m.groundRaycastDown;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 1f, m.castBoxWidth);
    private Quaternion WallSlideRotation = Quaternion.identity;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = FeetOrigin;
        Gizmos.DrawWireCube(origin, CastBox * m.groundCastRadius * 2);
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + DesiredVelocity * m.slideWallCastLength);

        if (hits.Length > 0)
        {
            Vector3 point = hits[0].point;
            Gizmos.DrawLine(point, point + hits[0].normal * 10);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(point, point + wallSlideVector);
        }
    }

    private void Awake()
    {
        if (!body) body = GetComponent<Rigidbody>();
        stateMachine = StateMachine<CharacterState>.Initialize(this);
        stateMachine.ManualUpdate = true;

        stateMachine.ChangeState(CharacterState.Grounded);
    }

    private void Update()
    {
        stateMachine.UpdateManually();
        CalculateHorizontalVelocity();

        isRunning = axis.magnitude != 0f;
    }

    private void CalculateHorizontalVelocity()
    {
        Vector2 usedAxis = IsinAir ? lastAxis : axis;
        float x = usedAxis.x;
        float z = usedAxis.y;
        Vector3 target = new Vector3(x, 0, z).normalized * m.runSpeed;
        Vector3 horiz = Vector3.SmoothDamp(velocity, target, ref currentVel, m.acceleration);
        velocity = new Vector3(horiz.x, velocity.y, horiz.z);
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        body.velocity = FinalVelocity;
    }

    public void InputAxis(Vector2 axis)
    {
        this.axis = axis;
        if(axis.magnitude != 0)
        {
            lastAxis = axis;
        }
    }

    private void Grounded_Enter()
    {
        yVelocity = 0f;
        ResetJumpCount();
        playerCamera.Land();
    }

    private Vector3 wallSlideVector;

    private void Grounded_Update()
    {
        if (!CastGround())
        {
            jumpLeft--;
            stateMachine.ChangeState(CharacterState.Falling);
        }

        CastWall();
        if (hits.Length > 0 && m.slideAgainstWalls)
        {
            Vector3 wallNormal = hits[0].normal;
            float angle = Vector3.SignedAngle(wallNormal, Forward, Vector3.up);
            float mul = angle >= 0 ? 1f : -1f;
            Vector3 cross = Vector3.Cross(mul * Vector3.up, wallNormal);

            float dot = Vector3.Dot(Forward.normalized, wallNormal.normalized);
            dot = Mathf.Clamp(dot, -1f, 0f);
            Debug.Log(dot);
            WallSlideRotation = Quaternion.AngleAxis(m.curveWallSlide.Evaluate(-dot) * m.maxSlideWallSpeed, Vector3.up);

            // wallSlideVector = cross.normalized * m.runSpeed * Mathf.Lerp(m.minSlideWallSpeed, m.maxSlideWallSpeed, dot);
        }

        else
            WallSlideRotation = Quaternion.identity;
    }

    private void ResetJumpCount()
    {
        jumpLeft = m.jumpCount;
    }

    public void Jump()
    {
        if (jumpLeft > 0)
        {
            jumpLeft--;
            stateMachine.ChangeState(CharacterState.Jumping);
            lastAxis = axis;
            if (m.resetVelocityOnJump) ResetVelocity();
        }
    }

    private void Jumping_Enter()
    {
        jumpProgress = 0f;
        playerCamera.Land();
    }

    private void Jumping_Update()
    {
        jumpProgress += Time.deltaTime * m.jumpProgressSpeed;
        yVelocity = m.jumpCurve.Evaluate(jumpProgress) * m.jumpStrength;
        if (jumpProgress >= 1f)
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }
    }

    private void Falling_Enter()
    {
        fallProgress = 0f;
    }

    private void Falling_Update()
    {
        fallProgress += Time.deltaTime * m.fallProgressSpeed;
        yVelocity = -m.fallCurve.Evaluate(fallProgress) * m.fallStrength;
        if (CastGround())
        {
            stateMachine.ChangeState(CharacterState.Grounded);
        }
    }

    public bool CastGround()
    {
        if (Physics.BoxCast(FeetOrigin, CastBox * m.groundCastRadius, -Vector3.up, out hit, Quaternion.identity, m.groundRaycastDown, m.groundMask))
        {
            //Debug.Log("Hit " + hit.collider.gameObject.name);
            SnapToGround();
            return true;
        }
        return false;
    }

    private void SnapToGround()
    {
        body.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    }

    private void ResetVelocity()
    {
        velocity = Vector3.zero;
        yVelocity = 0f;
    }

    public bool CastWall()
    {
        hits = Physics.RaycastAll(transform.position + Vector3.up, Forward, m.slideWallCastLength);
        return hits.Length > 0;
    }
}

public enum CharacterState
{
    Grounded,
    Jumping,
    Falling,
    WallClimbing
}