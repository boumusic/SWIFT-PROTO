using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using MonsterLove.StateMachine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody body;
    public Collider coll;
    public CharacterSettings m;
    public PlayerCamera playerCamera;
    public CharacterAnimator animator;
    public CharacterFeedbacks feedbacks;


    [Header("Visuals")]
    public GameObject tps;
    public GameObject flagVisuals;
    public Renderer flagVisualsRend;
    public GameObject fps;
    public bool startTps = false;
    private Player player;
    public string PlayerName => player != null ? player.PlayerName : "NPC";
    public Color TeamColor => player != null ? player.TeamColor : Color.white;

    #region Movement

    private Vector2 accelProgress;
    private Vector2 axis;
    public Vector2 Axis { get => axis;}

    private Vector3 DesiredVelocity => CamRotation * new Vector3(axis.x, 0, axis.y);
    private Vector3 velocity;
    private Vector3 currentVel;
    private bool isRunning;
    private float yVelocity;
    private bool spacebar;

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
    public bool Dashing => CurrentState == CharacterState.Dashing;
    public Vector3 FinalVelocity
    {
        get
        {
            if (grounded || Dashing) return CamRotation * Velocity;
            else return LastCamRotation * Velocity;

        }
    }
    public float FacingVelocity => Mathf.Clamp01(Vector3.Dot(Forward, FinalVelocity));
    public float CameraRoll => Vector3.Dot(Right, FinalVelocity.normalized) * (CurrentState == CharacterState.Jumping ? 3.5f : 1f);

    private Quaternion LastCamRotation;
    public Quaternion CamRotation => playerCamera.Forward;

    public bool IsRunning { get => isRunning; }
    public bool IsinAir => CurrentState == CharacterState.Jumping || CurrentState == CharacterState.Falling;

    public Vector3 FeetOrigin => transform.position + Vector3.up * m.groundRaycastUp;
    public Vector3 FeetDestination => FeetOrigin - Vector3.up * m.groundRaycastDown;
    public Vector3 CastBox => new Vector3(m.castBoxWidth, 1f, m.castBoxWidth);

    private bool grounded => CurrentState == CharacterState.Grounded;
    private float CurrentDecelSpeed => grounded ? m.decelerationSpeed : m.jumpDecelerationSpeed;
    private float CurrentAccelSpeed => grounded ? m.accelerationSpeed : m.jumpAccelerationSpeed;
    
    private float dashProgress = 0f;
    

    #endregion

    private bool isAttacking;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    private int healthPointsLeft;
    public Action OnAttack;
    public Action OnKill;
    public Action OnScore;
    public Action OnDash;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = FeetOrigin;
        //Gizmos.DrawWireCube(origin, CastBox * m.groundCastRadius * 2);
        //Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + DesiredVelocity * m.slideWallCastLength);

        if (hits.Length > 0)
        {
            //Vector3 point = hits[0].point;
            //Gizmos.DrawLine(point, point + hits[0].normal * 10);

            //Gizmos.color = Color.green;
            //Gizmos.DrawLine(point, point + wallSlideVector);

            Gizmos.DrawLine(transform.position, transform.position + WallUp * 10);
        }
    }

    private void Awake()
    {
        if (!body) body = GetComponent<Rigidbody>();
        stateMachine = StateMachine<CharacterState>.Initialize(this);
        stateMachine.ManualUpdate = true;

        stateMachine.ChangeState(CharacterState.Grounded);

        tps.SetActive(startTps);
        OnKill += UIManager.Instance.HitMarker;

        dashCooldownProgress = 1f;
    }

    private void Update()
    {
        stateMachine.UpdateManually();
        CalculateHorizontalVelocity();

        isRunning = axis.magnitude != 0f;
        animator.Run(isRunning, velocity.normalized);

        CheckWallClimb();
        OrientModel();
        DashCooldown();
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void Start()
    {
        UpdateColor();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    private float xAccel = 0f;
    private float zAccel = 0f;

    private void CalculateHorizontalVelocity()
    {
        if (axis.magnitude != 0) LastCamRotation = CamRotation;
        if (CurrentState != CharacterState.WallClimbing)
        {
            Vector2 usedAxis = CurrentState == CharacterState.Grounded ? axis : axis;
            Accel(ref xAccel, usedAxis.x);
            Accel(ref zAccel, usedAxis.y);
            Vector3 target = new Vector3(xAccel, 0, zAccel) * m.runSpeed * (HasFlag? m.flagMultiplier : 1);

            velocity = target;
            if (CurrentState == CharacterState.Dashing) velocity = new Vector3(dashVelocity.x, 0f, dashVelocity.z);
        }
    }

    private void Accel(ref float accel, float axis)
    {
        if (axis != 0)
        {
            accel += axis * Time.deltaTime * CurrentAccelSpeed;
        }

        else
        {
            accel /= CurrentDecelSpeed;
        }
        accel = Mathf.Clamp(accel, -1f, 1f);

        if (Mathf.Abs(accel) < 0.001f) accel = 0f;
    }

    private void SnapAccelToAxis()
    {
        if(axis.magnitude != 0)
        {
            xAccel = axis.x;
            zAccel = axis.y;
        }        
    }

    private void ResetVelocity()
    {
        velocity = Vector3.zero;
        yVelocity = 0f;
    }

    private void ApplyVelocity()
    {
        body.velocity = FinalVelocity;
    }

    #region Input

    public void InputAxis(Vector2 axis)
    {
        this.axis = axis;
    }

    public void InputSpacebar(bool space)
    {
        spacebar = space;
    }

    #endregion

    #region Ground

    private void Grounded_Enter()
    {
        TryStopCoyote();
        yVelocity = 0f;
        ResetJumpCount();
        animator.Land();
        animator.Grounded(true);
        if (m.resetDashOnLand) resetDash = true;
    }

    private Vector3 wallSlideVector;

    private Coroutine coyote;

    private void Grounded_Update()
    {
        if (!CastGround())
        {
            if (coyote == null) coyote = StartCoroutine(CoyoteTime());
            stateMachine.ChangeState(CharacterState.Falling);
        }

        CastWall();

        /*
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
        */
    }

    private void TryStopCoyote()
    {
        if (coyote != null) StopCoroutine(coyote);
    }

    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(m.coyoteTime);
        jumpLeft = 1;
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

    #endregion

    #region Jump

    public void Jump()
    {
        if (jumpLeft > 0 && CurrentState != CharacterState.Dashing)
        {
            stateMachine.ChangeState(CharacterState.Jumping);
        }
    }

    private void Jumping_Enter()
    {
        if(axis.magnitude != 0)
            LastCamRotation = CamRotation;

        jumpLeft--;
        SnapAccelToAxis();
        if (m.resetVelocityOnJump) ResetVelocity();
        jumpProgress = 0f;
        animator.Jump();
        animator.Grounded(false);
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

    private void ResetJumpCount()
    {
        jumpLeft = m.jumpCount;
    }

    #endregion

    #region Falling

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

    #endregion

    #region Dash

    private Vector3 dashVelocity;
    private Vector2 dashAxis;
    private bool resetDash = true;
    private bool cooldownDashDone = true;
    public bool CanDash => resetDash && cooldownDashDone;
    private float dashCooldownProgress = 1f;
    public float DashCooldownProgress => dashCooldownProgress;
    public bool ResetDash => resetDash;

    public void StartDash()
    {
        if (CanDash)
        {
            OnDash?.Invoke();
            stateMachine.ChangeState(CharacterState.Dashing);
        }
    }

    private void Dashing_Enter()
    {
        yVelocity = 0f;
        if (axis.magnitude != 0)
            dashAxis = axis;
        else
            dashAxis = new Vector2(0, 1);

        dashProgress = 0f;
        dashCooldownProgress = 0f;
        cooldownDashDone = false;
        resetDash = false;
        feedbacks.Play("Dash");
    }

    private void Dashing_Update()
    {
        dashProgress += Time.deltaTime * m.dashProgressSpeed;
        dashVelocity = new Vector3(dashAxis.x, 0f, dashAxis.y).normalized * m.dashCurve.Evaluate(dashProgress) * m.dashStrength;
        if (dashProgress >= 1f)
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }
    }

    private void DashCooldown()
    {
        if (!cooldownDashDone)
        {
            if (dashCooldownProgress < 1f)
            {
                dashCooldownProgress += Time.deltaTime / m.dashCooldown;
            }

            else
            {
                cooldownDashDone = true;
            }
        }
    }

    #endregion

    #region Wall

    private void CheckWallClimb()
    {
        if (CastWall())
        {
            if (CurrentState != CharacterState.WallClimbing && spacebar)
            {
                stateMachine.ChangeState(CharacterState.WallClimbing);
            }
        }
    }

    private void WallClimbing_Enter()
    {
        if (m.resetDashOnWallclimb) resetDash = true;
        animator.WallClimb(true);
        jumpLeft++;
    }

    private Vector3 WallNormal => hits.Length > 0 ? hits[0].normal : Vector3.zero;
    private Vector3 WallUp => Vector3.Cross(WallNormal, -Right);

    private void WallClimbing_Update()
    {
        if (!spacebar || !CastWall())
        {
            //stateMachine.ChangeState(CharacterState.Falling);
            jumpLeft++;
            Jump();
        }

        yVelocity = m.wallClimbSpeed;
        velocity = Vector3.zero;
    }

    private void WallClimbing_Exit()
    {
        animator.WallClimb(false);
    }

    public bool CastWall()
    {
        RaycastHit[] down = Physics.RaycastAll(transform.position, DesiredVelocity, m.slideWallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
        RaycastHit[] up = Physics.RaycastAll(transform.position + Vector3.up * 2f, DesiredVelocity, m.slideWallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
        List<RaycastHit> final = new List<RaycastHit>();
        for (int i = 0; i < down.Length; i++)
        {
            final.Add(down[i]);
        }

        for (int i = 0; i < up.Length; i++)
        {
            final.Add(up[i]);
        }

        hits = final.ToArray();
        return hits.Length > 0;
    }

    #endregion

    #region Health

    private void ResetHealth()
    {
        healthPointsLeft = m.healthPoints;
    }

    public void TakeDamage(int damage)
    {
        healthPointsLeft -= damage;

        if (healthPointsLeft <= 0)
        {
            healthPointsLeft = 0;
            Die();
        }
    }

    private bool isDead = false;

    public void Die()
    {
        if (!isDead)
        {
            DropFlag();
            ResetVelocity();
            flagVisuals.SetActive(false);
            coll.enabled = false;
            healthPointsLeft = 0;
            isDead = true;
            animator.Death();
            RespawnManager.Instance.Death(this);
            StartCoroutine(DeathAnim());
        }
    }

    private IEnumerator DeathAnim()
    {
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
        Disable();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        isDead = false;
    }

    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
        Respawn();
    }

    #endregion

    #region Attack

    public bool CanAttack => CurrentState != CharacterState.WallClimbing && !isAttacking && !HasFlag && !Dashing;
    public void TryAttack()
    {
        if (CanAttack)
        {
            animator.Attack();
            StartCoroutine(AttackDelay());
        }
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m.attackDelay);
        OnAttack?.Invoke();
        isAttacking = true;
        StartCoroutine(AttackDuration());
        Vector3 center = transform.position + Vector3.up * 1.8f + playerCamera.transform.forward * m.attackLength / 2f;
        Vector3 halfExtents = new Vector3(m.attackWidth, m.attackHeight, m.attackLength) / 2f;
        RaycastHit[] hits = Physics.BoxCastAll(center, halfExtents, Vector3.forward, Quaternion.identity, m.attackLength);

        if (hits.Length > 0)
        {
            if (NetworkManager.Instance != null)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    NetworkedPlayer player;
                    if (hits[i].collider.transform.root.TryGetComponent(out player))
                    {
                        NetworkedPlayer myNetworkerPlayer = transform.root.GetComponent<NetworkedPlayer>();

                        if (player.player == null && (player.teamIndex != myNetworkerPlayer.teamIndex))
                        {
                            Debug.Log("sending kill rpc");

                            player.networkObject.SendRpc(NetworkedPlayerBehavior.RPC_TRY_HIT, Receivers.Server, 
                                myNetworkerPlayer.playerName, myNetworkerPlayer.teamIndex);

                            UIManager.Instance.HitMarker();
                        }
                    }
                }

                // don't do the "normal" hit dectection
                yield return null;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                Character chara;
                if (hits[i].collider.gameObject.TryGetComponent(out chara))
                {
                    if (chara != this)
                    {
                        if (!chara.isDead)
                        {
                            chara.TakeDamage(m.damage);
                            UIManager.Instance.DisplayKillFeed(this, chara);
                            UIManager.Instance.HitMarker();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(m.attackDuration);
        isAttacking = false;
    }

    #endregion

    #region TPS

    private bool isTPS = false;
    public void ToggleTPS()
    {
        bool newTPS = !isTPS;
        isTPS = newTPS;
        playerCamera.ToggleTPS(newTPS);

        fps.SetActive(!newTPS);
        tps.SetActive(newTPS);
    }

    private void OrientModel()
    {
        if (Forward != Vector3.zero)
        tps.transform.forward = Forward;
    }

    #endregion

    #region CTF

    private Flag flag = null;
    public Flag Flag { get => flag; }
    public int TeamIndex => player.TeamIndex;
    public bool HasFlag => flag != null;

    public void Capture(Flag flag)
    {
        flag.gameObject.SetActive(false);
        flagVisuals.SetActive(true);
        UIManager.Instance.LogMessage(PlayerName + " captured the Flag !");
        this.flag = flag;
    }

    public void Score()
    {
        string message = PlayerName + " scored for team " + TeamIndex + "!";
        UIManager.Instance.LogMessage(message);
        ResetFlag();
        TeamManager.Instance.Score(TeamIndex);
        CTFManager.Instance.OnTeamScored?.Invoke();
        OnScore?.Invoke();
    }

    private void DropFlag()
    {
        if(HasFlag)
        {
            string message = "The flag has been retreived!";
            UIManager.Instance.LogMessage(message);
            ResetFlag();
        }
    }

    private void ResetFlag()
    {
        flag.gameObject.SetActive(true);
        flagVisuals.SetActive(false);
        flag = null;
    }

    #endregion

    #region Visuals

    public void UpdateColor()
    {
        GetComponentInChildren<SkinnedMeshRenderer>(true).material.SetColor("_Color", TeamColor);
        if(player)
            flagVisualsRend.material.SetColor("_Color", TeamManager.Instance.GetOppositeTeamColor(player.TeamIndex));
    }

    #endregion
}

public enum CharacterState
{
    Grounded,
    Jumping,
    Falling,
    WallClimbing,
    Dashing
}