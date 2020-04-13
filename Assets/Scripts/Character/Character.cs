using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class Character : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody body;
    public Collider coll;
    public CharacterSettings m;
    public PlayerCamera playerCamera;
    public CharacterAnimator animator;
    public CharacterFeedbacks feedbacks;
    public Propeller propeller;

    [Header("Visuals")]
    public GameObject tps;
    public GameObject ragdollPrefab;
    public GameObject[] flagVisuals;
    public Renderer[] flagVisualsRend;
    public GameObject sword;
    public bool startTps = false;
    private Player player;
    private NetworkedPlayer nPlayer;
    public NetworkedPlayer NPlayer { get { if (nPlayer == null) nPlayer = GetComponentInParent<NetworkedPlayer>(); return nPlayer; } }
    public string PlayerName => player != null ? player.PlayerName : "NPC";

    public Color TeamColor => player != null ? player.TeamColor : Color.white;

    #region Movement

    private Vector2 axis;
    public Vector2 Axis { get => axis; }

    private Vector3 DesiredDirection => CamRotation * new Vector3(axis.x, 0, axis.y);
    private Vector3 velocity;
    private bool isRunning;
    private float yVelocity;
    private bool spacebar;

    private float jumpProgress = 0f;
    private float fallProgress = 0f;
    private int jumpLeft;
    private RaycastHit hit;
    private RaycastHit[] hits = new RaycastHit[5];

    private StateMachine<CharacterState> stateMachine;
    public CharacterState CurrentState => stateMachine != null ? stateMachine.State : CharacterState.Grounded;
    public Vector3 Velocity => new Vector3(velocity.x, yVelocity, velocity.z);

    public Vector3 Forward => new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
    public Vector3 Right => new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z);

    public Vector3 FinalVelocity
    {
        get
        {
            if (propeller.IsPropelling) return propeller.Velocity();
            else if (grounded) return CamRotation * Velocity;
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
    private float CurrentDecelAmount => grounded ? m.decelerationAmount : m.jumpDecelerationAmount;
    private float CurrentAccelTime => grounded ? m.accelerationTime : m.jumpAccelerationTime;

    private float dashProgress = 0f;

    #endregion

    public Action OnAttack;
    public Action OnKill;
    public Action OnScore;
    public Action OnStartDash;
    public Action OnDashReady;
    public Action OnStartWallclimb;
    public Action OnWallclimbReady;

    private bool local => NetworkedGameManager.Instance == null;

    public float Radius => (coll as CapsuleCollider).radius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position;
        Gizmos.DrawLine(origin, origin + WallNormal * 5);
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
        Animations();
        CheckWallClimb();
        OrientModel();
        DashCooldown();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            Knockbacked((-Forward + Vector3.up).normalized);
        }

#endif
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

    #region Velocity

    private float xAccel = 0f;
    private float zAccel = 0f;
    private float CurrentRunSpeed => m.runSpeed * (HasFlag ? m.flagMultiplier : 1);

    private void CalculateHorizontalVelocity()
    {
        if (axis.magnitude != 0) LastCamRotation = CamRotation;

        Vector2 usedAxis = axis;
        Accel(ref xAccel, usedAxis.x, usedAxis);
        Accel(ref zAccel, usedAxis.y, usedAxis);
        Vector3 target = new Vector3(xAccel, 0, zAccel) * CurrentRunSpeed;
        if (CurrentState == CharacterState.WallClimbing) target *= m.wallClimbHorizSpeedMultiplier;
        velocity = target;
    }

    private void Accel(ref float accel, float axis, Vector2 fullVector)
    {
        if (axis != 0)
        {
            accel += axis * Time.deltaTime * (1 / CurrentAccelTime);
        }

        else
        {
            accel /= CurrentDecelAmount;
        }

        float magnitude = fullVector.magnitude;
        float normalizedMax = 1 / magnitude;

        accel = Mathf.Clamp(accel, -normalizedMax, normalizedMax);

        if (Mathf.Abs(accel) < 0.001f) accel = 0f;
    }

    private void SnapAccelToAxis()
    {
        if (axis.magnitude != 0)
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

    #endregion

    #region Input

    public void InputAxis(Vector2 axis)
    {
        this.axis = axis;
    }

    private float spacebarCharge = 0f;
    private bool wasSpacebar = false;
    public void InputSpacebar(bool space)
    {
        wasSpacebar = spacebar;
        spacebar = space;


        if (CurrentState != CharacterState.Jumping)
        {
            if (space)
            {
                spacebarCharge += Time.deltaTime * m.jumpChargeSpeed;
            }
        }

        spacebarCharge = Mathf.Clamp01(spacebarCharge);

        if (wasSpacebar && !spacebar && hasReleasedJump)
        {
            hasReleasedJump = true;
            Jump(spacebarCharge < 0.5f);
            spacebarCharge = 0f;
        }

        if (space)
        {
            hasReleasedJump = false;
        }

        if (spacebarCharge > 1f)
        {

        }
    }

    #endregion

    #region Ground

    private void Grounded_Enter()
    {
        TryStopCoyote();
        yVelocity = 0f;
        ResetJumpCount();

        if (shouldLandAnim)
        {
            animator.Land();
        }

        else
            shouldLandAnim = true;

        animator.Grounded(true);
        ResetWallclimb();
    }

    private void ResetWallclimb()
    {
        wallClimbProgress = 0f;
        canUseWallClimbThreshold = true;
        OnWallclimbReady?.Invoke();
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
            //Debug.Log("Hit " + hit.collider.gameObject.name, hit.collider.gameObject);
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
    private bool hasReleasedJump = true;
    private bool shortJump = false;
    private float CurrentJumpStrength => shortJump ? m.shortJumpStrength : m.jumpStrength;
    private bool isJumping => CurrentState == CharacterState.Jumping;
    private Vector3 wallJumpDir = Vector3.zero;

    public void Jump(bool shortJump = false)
    {
        if (CurrentState == CharacterState.WallSliding)
        {
            WallJump();
            return;
        }

        if (jumpLeft > 0 && !isDashing && !isImpulsing)
        {
            this.shortJump = false;

            if (wallSliding) wallJumpDir = WallNormal;
            else wallJumpDir = Vector3.zero;

            stateMachine.ChangeState(CharacterState.Jumping);
        }
    }

    private void Jumping_Enter()
    {
        if (axis.magnitude != 0)
            LastCamRotation = CamRotation;

        animator.Jump(jumpLeft < 2);
        jumpLeft--;
        SnapAccelToAxis();
        if (m.resetVelocityOnJump) ResetVelocity();
        jumpProgress = 0f;
        animator.Grounded(false);
    }

    private void Jumping_Update()
    {
        jumpProgress += Time.deltaTime * m.jumpProgressSpeed;
        yVelocity = m.jumpCurve.Evaluate(jumpProgress) * CurrentJumpStrength;
        if (wallJumpDir.magnitude != 0) velocity = wallJumpDir * yVelocity;
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

    private float fallInitVelocityY;

    private void Falling_Enter()
    {
        Debug.Log("Start Falling");
        fallInitVelocityY = body.velocity.y;
        fallProgress = 0f;
    }

    private void Falling_Update()
    {
        if (!propeller.IsPropelling)
        {
            fallProgress += Time.deltaTime * m.fallProgressSpeed;
            yVelocity = Mathf.Lerp(fallInitVelocityY, -m.fallStrength, Mathf.Clamp01(m.fallCurve.Evaluate(fallProgress)));
            if (CastGround())
            {
                stateMachine.ChangeState(CharacterState.Grounded);
            }

            if (CastWall())
            {
                stateMachine.ChangeState(CharacterState.WallSliding);
            }
        }
    }

    private void Falling_Exit()
    {
        yVelocity = 0f;
        fallProgress = 0f;
    }

    #endregion

    #region Dash

    public bool isDashing;
    public bool InDashMovement => isDashing && dashProgress < 0.5f;
    private Vector3 dashAxis;
    private bool cooldownDashDone = true;
    public bool CanDash => cooldownDashDone && !isImpulsing;
    private float dashCooldownProgress = 1f;
    public float DashCooldownProgress => dashCooldownProgress;
    private float CurrentDashCooldown => HasFlag ? m.dashFlagCooldown : m.dashCooldown;

    public void StartDash()
    {
        //Debug.Log(CanDash);
        if (CanDash)
        {
            if (axis.magnitude != 0)
                dashAxis = new Vector3(axis.x, 0, axis.y);
            else
                dashAxis = new Vector3(0, 0, 1f);

            propeller.RegisterPropulsion(CamRotation * dashAxis, m.dash, EndDash);
            isDashing = true;
            OnStartDash?.Invoke();

            dashCooldownProgress = 0f;
            cooldownDashDone = false;
            feedbacks.Play("Dash");
            animator.Dash();
        }
    }

    private void EndDash()
    {
        isDashing = false;
        stateMachine.ChangeState(CharacterState.Falling);
    }

    private void DashCooldown()
    {
        if (!cooldownDashDone)
        {
            if (dashCooldownProgress < 1f)
            {
                dashCooldownProgress += Time.deltaTime / CurrentDashCooldown;
            }

            else
            {
                OnDashReady?.Invoke();
                cooldownDashDone = true;
            }
        }
    }

    #endregion

    #region Wallclimb

    private Vector3 WallNormal => hits.Length > 0 ? hits[0].normal : Vector3.zero;
    private Vector3 WallUp => Vector3.Cross(WallNormal, -Right);
    private float wallClimbProgress;
    public float WallClimbCharge => 1 - wallClimbProgress;
    private bool canWallClimb => WallClimbCharge > 0f;
    private bool canUseWallClimbThreshold = true;

    private void CheckWallClimb()
    {
        if(CurrentState != CharacterState.WallClimbing)
        {
            if (CastWall() && canWallClimb || CastLedge())
            {
                if (spacebar)
                {
                    stateMachine.ChangeState(CharacterState.WallClimbing);
                }
            }
        }
    }

    private void WallClimbing_Enter()
    {
        animator.WallClimb(true);
        //SnapToWall();
        OnStartWallclimb?.Invoke();
    }

    private void SnapToWall()
    {
        if (hits.Length > 0)
        {
            Vector3 hitPos = hits[0].point + hits[0].normal * Radius;
            transform.position = new Vector3(hitPos.x, transform.position.y, hitPos.z);
        }
    }

    private void WallClimbing_Update()
    {
        wallClimbProgress += Time.deltaTime / m.wallClimbDuration;
        yVelocity = m.wallClimbSpeed * m.curveWallClimb.Evaluate(wallClimbProgress);

        if (!spacebar || !CastWall())
        {
            jumpLeft++;
            Jump();

            /*
            //Threshold when accidentally wallclimbing a small step and not getting the ground reset
            if (wallClimbProgress > m.wallClimbConsumeThreshold)
            {
                canWallClimb = false;
            }

            else
            {
                if (!canUseWallClimbThreshold)
                    canWallClimb = false;
                else
                    canUseWallClimbThreshold = false;
            }
            */
        }

        if (!canWallClimb && !CastLedge())
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }

        //velocity = Vector3.zero;
    }

    private void WallClimbing_Exit()
    {
        animator.WallClimb(false);
    }

    private float CurrentWallCastLength => m.wallCastLength * axis.magnitude;

    public bool CastWall()
    {
        Vector3 origin = transform.position + Vector3.up * 0.05f /*+ Forward * Radius*/;
        RaycastHit[] down = Physics.RaycastAll(origin, Forward, CurrentWallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
        RaycastHit[] up = Physics.RaycastAll(origin, Forward, CurrentWallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
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
        //hits = Physics.CapsuleCastAll(transform.position, transform.position + Vector3.up, 0.1f, DesiredVelocity, m.slideWallCastLength);
        return hits.Length > 0;
    }

    public bool CastLedge()
    {
        RaycastHit[] down = Physics.RaycastAll(transform.position + Vector3.up * m.ledgeCastMinHeight, Forward, m.wallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
        RaycastHit[] up = Physics.RaycastAll(transform.position + Vector3.up * m.ledgeCastMaxHeight, Forward, m.wallCastLength, m.groundMask, QueryTriggerInteraction.Ignore);
        bool cast = down.Length > 0 && up.Length == 0;
        if (cast) Debug.Log("Ledge");
        return cast;
    }

    #endregion

    #region WallSlide

    private bool wallSliding => CurrentState == CharacterState.WallSliding;
    private float wallSlideTime = 0f;

    private void WallSliding_Enter()
    {
        wallSlideTime = Time.time;
    }

    private void WallSliding_Update()
    {
        yVelocity = Mathf.Lerp(yVelocity, -m.wallSlideSpeed, Mathf.Clamp01(Time.time - wallSlideTime));
        if (!CastWall())
        {
            stateMachine.ChangeState(CharacterState.Falling);
        }

        if (CastGround())
        {
            stateMachine.ChangeState(CharacterState.Grounded);
        }
    }

    #endregion

    #region WallJump

    private bool isWallJumping = false;
    private void WallJump()
    {
        isWallJumping = true;
        Vector3 dir = new Vector3(WallNormal.x, 1, WallNormal.z);
        propeller.RegisterPropulsion(dir, m.wallJump, EndWallJump);
    }

    private void EndWallJump()
    {
        isWallJumping = false;
        stateMachine.ChangeState(CharacterState.Falling);
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
            ToggleFlagVisuals(true);
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

    private bool isAttacking;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    private int healthPointsLeft;
    public bool CanAttack => CurrentState != CharacterState.WallClimbing && !isAttacking && !HasFlag && !InDashMovement && !isImpulsing && cooldownAttackDone;
    private bool cooldownAttackDone = true;
    private Coroutine attackDelay;
    private Coroutine attackDuration;
    private float impulseProgress = 0f;
    private Vector3 impulseVelocity;
    private Vector3 impulseDir;
    private bool isImpulsing;
    private bool shouldLandAnim = true;

    public void CancelAttack()
    {
        if (attackDelay != null) StopCoroutine(attackDelay);
        if (attackDuration != null) StopCoroutine(attackDuration);
        isStartingAttack = false;
        isAttacking = false;
        cooldownAttackDone = true;
    }

    public void TryAttack()
    {
        if (CanAttack)
        {
            if (!isTPS) sword.SetActive(true);

            animator.Attack();
            if (attackDelay != null) StopCoroutine(attackDelay);
            attackDelay = StartCoroutine(AttackDelay());

            if (grounded)
            {
                Debug.Log(Forward);
                propeller.RegisterPropulsion(Forward, m.attackImpulse, EndImpulse);
                isImpulsing = true;
            }
        }
    }

    private void EndImpulse()
    {
        isImpulsing = false;
    }

    [HideInInspector] public bool isStartingAttack;
    private IEnumerator AttackDelay()
    {
        cooldownAttackDone = false;
        isStartingAttack = true;

        yield return new WaitForSeconds(m.attackDelay);

        OnAttack?.Invoke();
        isAttacking = true;
        if (attackDuration != null) StopCoroutine(attackDuration);
        attackDuration = StartCoroutine(AttackDuration());

        isStartingAttack = false;

        if (isKnockbacked) yield return null;

        NetworkedPlayer myNetworkerPlayer = transform.root.GetComponent<NetworkedPlayer>();
        if (NetworkManager.Instance != null)
        {
            if (!myNetworkerPlayer.networkObject.alive) yield return null;
        }

        Vector3 center = transform.position + m.attackCenter + playerCamera.transform.forward * m.attackLength / 2f;
        Vector3 halfExtents = new Vector3(m.attackWidth, m.attackSizeY, m.attackLength) / 2f;
        RaycastHit[] hits = Physics.BoxCastAll(center, halfExtents, Vector3.forward, CamRotation, m.attackLength);

        if (hits.Length > 0)
        {
            if (NetworkManager.Instance != null)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    NetworkedPlayer player;
                    if (hits[i].collider.transform.root.TryGetComponent(out player))
                    {
                        if (player.player == null && (player.teamIndex != myNetworkerPlayer.teamIndex))
                        {
                            Debug.Log("sending kill rpc");

                            player.networkObject.SendRpc(NetworkedPlayerBehavior.RPC_TRY_HIT, Receivers.Server,
                                myNetworkerPlayer.networkObject.NetworkId, myNetworkerPlayer.playerName, myNetworkerPlayer.teamIndex, myNetworkerPlayer.playerCamera.transform.forward);

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

        yield return new WaitForSeconds(m.attackCooldown);

        cooldownAttackDone = true;
    }

    public void DisableWeapon()
    {
        if (!isTPS) sword.SetActive(false);
    }

    #endregion

    #region Knockback

    private Vector3 kbDir;
    private bool isKnockbacked;

    public void Knockbacked(Vector3 direction)
    {
        kbDir = new Vector3(direction.x, 0, direction.z).normalized;
        propeller.RegisterPropulsion(kbDir, m.knockback, EndKnockback);
        feedbacks.Play("Parried");
        isKnockbacked = true;
    }

    private void EndKnockback()
    {
        isKnockbacked = false;
        stateMachine.ChangeState(CharacterState.Falling);
    }

    #endregion

    #region TPS

    private bool isTPS = false;
    public void ToggleTPS()
    {
        bool newTPS = !isTPS;
        isTPS = newTPS;
        playerCamera.ToggleTPS(newTPS);
        DisableWeapon();
        tps.SetActive(newTPS);
    }

    private void OrientModel()
    {
        if (Forward != Vector3.zero)
        {
            if (CurrentState == CharacterState.WallClimbing && hits.Length > 0)
            {
                tps.transform.forward = -hits[0].normal;
            }

            else
            {
                tps.transform.forward = Forward;
            }
        }
    }

    #endregion

    #region CTF

    private Flag flag = null;
    public Flag Flag { get => flag; }
    public int TeamIndex => player == null ? -1 : player.TeamIndex;
    public bool HasFlag { get { if (NPlayer != null) return NPlayer.networkObject.hasFlag; else return flag != null; } }

    public int JumpLeft { get => jumpLeft; set => jumpLeft = value; }

    public void Capture(Flag flag)
    {
        this.flag = flag;
        ToggleFlagVisuals(true);
        if (local)
        {
            UIManager.Instance.LogMessage(PlayerName + " captured the Flag !");
            flag.gameObject.SetActive(false);
        }
    }

    public void Score()
    {
        string message = PlayerName + " scored for team " + TeamIndex + "!";
        OnScore?.Invoke();
        ResetFlag();

        if (local)
        {
            UIManager.Instance.LogMessage(message);
            TeamManager.Instance.Score(TeamIndex);
            CTFManager.Instance.OnTeamScored?.Invoke();
        }
    }

    private void DropFlag()
    {
        if (HasFlag)
        {
            string message = "The flag has been retreived!";
            UIManager.Instance.LogMessage(message);
            ResetFlag();
        }
    }

    private void ResetFlag()
    {
        ToggleFlagVisuals(false);
        if (local) flag.gameObject.SetActive(true);
        flag = null;
    }

    public void ToggleFlagVisuals(bool value)
    {
        for (int i = 0; i < flagVisuals.Length; i++)
        {
            flagVisuals[i].SetActive(value);
        }
    }

    #endregion

    #region Visuals

    public void UpdateColor()
    {
        GetComponentInChildren<SkinnedMeshRenderer>(true).material.SetColor("_Color", TeamColor);
        if (player)
        {
            for (int i = 0; i < flagVisualsRend.Length; i++)
            {
                flagVisualsRend[i].material.SetColor("_Color", TeamManager.Instance.GetOppositeTeamColor(player.TeamIndex));
            }
        }
    }

    private Vector3 currentVelAnim;
    private Vector3 animVelocity;
    private float smoothAnim = 0.05f;

    private void Animations()
    {
        isRunning = velocity.magnitude != 0f;
        Vector3 target = velocity / CurrentRunSpeed;
        if (target.z != 0)
        {
            if (target.x > 0) target.x = Mathf.Sign(target.x);
            else target.x = 0;
            target.z = Mathf.Sign(target.z);
        }
        animVelocity = Vector3.SmoothDamp(animVelocity, target, ref currentVelAnim, smoothAnim);

        if (velocity.magnitude != 0)
        {
            animator.Velocity(animVelocity);
        }

        animator.Run(isRunning);
        animator.IsFalling(CurrentState == CharacterState.Falling);
        animator.Jumping(isJumping);
    }

    #endregion
}

public enum CharacterState
{
    Grounded,
    Jumping,
    WallClimbing,
    Impulsing,
    WallSliding,
    Falling
}