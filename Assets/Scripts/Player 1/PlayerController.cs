using KBCore.Refs;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

/// <summary>
/// Main player controller. Initializes player state, manages input-driven state machine,
/// coordinates animation/audio/particles, applies damage/healing, and handles respawn logic.
/// This class wires together stats, checks, sounds, and UI updates for the player.
/// </summary>
public class PlayerController : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] Rigidbody2D _rb;
    [SerializeField, Self] Collider2D _collider;
    [SerializeField, Anywhere] InputReader _input;
    [SerializeField, Anywhere] PlayerStatsBlack _playerStatsBlack;
    [SerializeField, Anywhere] PlayerRedStats _playerRedStats;
    [SerializeField, Self] CinemachineImpulseSource _impulseSource;
    [SerializeField, Self] Animator _animator;
    [SerializeField, Self] SpringJoint2D _springJoint;
    [SerializeField, Self] LineRenderer _lineRenderer;
    [SerializeField, Self] SpriteRenderer _spriteRenderer;
    [SerializeField, Anywhere] Transform _playerCenter;
    [SerializeField, Self] PlayerSounds _playerSounds;
    [SerializeField, Anywhere] Transform _hook;
    [SerializeField] ParticleSystem _jumpParticles;
    [SerializeField] ParticleSystem _dashParticles;
    [SerializeField] ParticleSystem _slidingParticles;
    [SerializeField] ParticleSystem[] _landingParticles;
    [SerializeField] ParticleSystem _deathBlackParticles;
    [SerializeField] ParticleSystem _deathRedParticles;
    [SerializeField] ParticleSystem _changeFromBlackParticles;
    [SerializeField] ParticleSystem _changeFromRedParticles;
    [SerializeField] LevelSetupObject _levelSetupObject;
    [SerializeField] InGameController _inGameController;
    [SerializeField, Anywhere] InputHandler _inputHandler;
    [SerializeField, Anywhere] PlayableDirector _deathCutScene;
    [SerializeField, Anywhere] PlayableDirector _respawnCutScene;
    [SerializeField, Anywhere] AudioListener _audioListener;


    [Header("Settings")]
    [SerializeField] bool _isFacingRight = true;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] LayerMask _triggerLayer;

    private PlayerAnimationController _animationController;
    private PlayerChecks _playerChecks;

    public bool _playingAttackAnimation = false;


    public PlayableDirector RespawnCutScene => _respawnCutScene;
    public AudioListener AudioListener => _audioListener;
    public ParticleSystem ChangeFromBlackParticles => _changeFromBlackParticles;
    public ParticleSystem ChangeFromRedParticles => _changeFromRedParticles;
    public LayerMask PlayerLayer => _playerLayer;
    public LayerMask TriggerLayer => _triggerLayer;
    public PlayerAnimationController AnimationController => _animationController;
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value; }
    public ParticleSystem JumpParticles => _jumpParticles;
    public ParticleSystem SlidingParticles => _slidingParticles;
    public ParticleSystem DashParticles => _dashParticles;
    public ParticleSystem[] LandingParticles => _landingParticles;
    public LineRenderer LineRenderer => _lineRenderer;  
    public Rigidbody2D RB => _rb;
    public Animator Animator => _animator;
    public Collider2D Collider => _collider;
    public InputReader Input => _input;
    public PlayerStatsBlack PlayerStatsBlack => _playerStatsBlack;
    public PlayerRedStats PlayerRedStats => _playerRedStats;
    public CinemachineImpulseSource ImpulseSource => _impulseSource;
    public SpringJoint2D SpringJoint => _springJoint;
    public Transform PlayerCenter => _playerCenter;
    public Transform Hook => _hook;

    private Timer[] _timers;

    StateMachine _stateMachine;
    public StateContext StateContext { get; private set; }
    public PlayerChecks PlayerChecks => _playerChecks;
    public PlayerSounds PlayerSounds => _playerSounds;


    public bool IsGrounded = false;


    private void Awake()
    {
        _animationController = new PlayerAnimationController(this);
        _playerChecks = new PlayerChecks(this);

        // Allow camera impulses to ignore Time.timeScale so shakes still play during hitstop/cutscenes
        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;

        // Prepare the state context with default suit (Black)
        StateContext = new StateContext();
        StateContext.IsBlack = true;

        // Hide the cursor for gameplay
        Cursor.visible = false;

        // Ensure swing joint is disabled until used
        SpringJoint.enabled = false;

        _rb.freezeRotation = true;

        // Cache the default gravity so states can temporarily modify and restore it
        StateContext.GravityScaleCached = _rb.gravityScale;

        // Create countdown timers from configured stats (jump buffer, coyote, dash, etc)
        StateContext.JumpBufferTimer = new CountdownTimer(_playerStatsBlack.JumpBufferTime);
        StateContext.JumpCoyoteTimer = new CountdownTimer(_playerStatsBlack.CoyoteTime);
        StateContext.ApexHangTimer = new CountdownTimer(_playerStatsBlack.ApexHangTime);
        StateContext.SideDashTimer = new CountdownTimer(_playerStatsBlack.DashTime);
        StateContext.DownDashTimer = new CountdownTimer(_playerRedStats.DashTime);
        StateContext.WallTouchedBuffer = new CountdownTimer(_playerStatsBlack.WallTouchBufferTime);
        StateContext.HorizontalMoveBlockTimer = new CountdownTimer(_playerStatsBlack.HorizontalMoveBlockTime); 
        StateContext.CombatBufferTimer = new CountdownTimer(_playerStatsBlack.CombatBufferTime);
        StateContext.AfterImageTimer = new CountdownTimer(_playerStatsBlack.TimeBetweenAfterImages);
        StateContext.GlidingBeforeResetTimer = new CountdownTimer(0.4f);

        // Collect timers for batch updates in UpdateTimers
        _timers = new Timer[] {StateContext.GlidingBeforeResetTimer, StateContext.JumpBufferTimer, StateContext.JumpCoyoteTimer, StateContext.ApexHangTimer, StateContext.SideDashTimer, StateContext.WallTouchedBuffer, StateContext.HorizontalMoveBlockTimer, StateContext.CombatBufferTimer, StateContext.DownDashTimer, StateContext.AfterImageTimer};  


        MeleeAttackState movingState = new MeleeAttackState(this, _playerStatsBlack, StateContext);
        DashState dashState = new DashState(this, PlayerStatsBlack, StateContext);
        GlidingState glidingState = new GlidingState(this, PlayerStatsBlack, StateContext);
        WallSlidingState wallSlidingState = new WallSlidingState(this, PlayerStatsBlack, StateContext);
        SwingState swingState = new SwingState(this, _playerRedStats, StateContext);
        DownDashState downDashState = new DownDashState(this, _playerRedStats, StateContext);
        PullingState pullingState = new PullingState(this, _playerRedStats, StateContext);

        _stateMachine = new StateMachine(movingState);

        _stateMachine.AddTransition(dashState, movingState, new FuncPredicate(() => !StateContext.SideDashTimer.isRunning));
        _stateMachine.AddAnyTransition(dashState, new FuncPredicate(() =>StateContext.SideDashesUsed < _playerStatsBlack.SideDashesAllowed && StateContext.SideDashTimer.isRunning));
        _stateMachine.AddTransition(movingState, glidingState, new FuncPredicate(() => StateContext.IsGliding && !StateContext.IsGrounded));
        _stateMachine.AddTransition(glidingState, movingState, new FuncPredicate(() => !StateContext.IsBlack || !StateContext.IsGliding || StateContext.IsGrounded));
        _stateMachine.AddTransition(movingState, wallSlidingState, new FuncPredicate(() => !StateContext.HorizontalMoveBlockTimer.isRunning && _playerChecks.CanSlide()));
        _stateMachine.AddTransition(wallSlidingState, movingState, new FuncPredicate(() => !_playerChecks.CanSlide() || !StateContext.IsSliding));
        _stateMachine.AddAnyTransition(swingState, new FuncPredicate(() => StateContext.IsSwinging && !_springJoint.enabled));
        _stateMachine.AddTransition(swingState, movingState, new FuncPredicate(() => StateContext.IsBlack || StateContext.JumpBufferTimer.isRunning || !StateContext.IsSwinging ));
        _stateMachine.AddAnyTransition(downDashState, new FuncPredicate(() => StateContext.DownDashesUsed < _playerRedStats.DownDashesAllowed && StateContext.DownDashTimer.isRunning && !StateContext.IsGrounded));
        _stateMachine.AddTransition(downDashState, movingState, new FuncPredicate(() => !StateContext.DownDashTimer.isRunning));
        _stateMachine.AddAnyTransition(pullingState, new FuncPredicate(() => StateContext.IsPulling && StateContext.IsTouchingWall == 0));
        _stateMachine.AddTransition(pullingState, movingState, new FuncPredicate(() => !StateContext.IsPulling || StateContext.IsTouchingWall != 0));
    }

    private void Start()
    {
        // Initialize animation controller (this binds animator hashes and sets initial animation state)
        AnimationController.Init();
    }

    /// <summary>
    /// Set the player's current health and update the HUD health bar (normalized value).
    /// </summary>
    public void SetHealth(int health)
    {
        StateContext.CurrentHealth = health;
        _inGameController.SetHealth(StateContext.CurrentHealth / (float)PlayerStatsBlack.MaxHealth);
    }

    private void Update()
    {
        // Handle rotation checks that depend on input/velocity and update all timers
        _playerChecks.HandleRotation();
        UpdateTimers(Time.deltaTime);

        // Update the state machine (frame-based logic)
        _stateMachine.Update();

        // Update HUD dash indicator: active when no side dashes have been used
        if (StateContext.SideDashesUsed == 0)
        {
            _inGameController.SetDashActive(true);
        }
        else { 
            _inGameController.SetDashActive(false);
        }

        if (StateContext.JumpCount >= PlayerStatsBlack.JumpsAllowed)
        {
            _inGameController.SetJumpActive(false);
        }
        else
        {
            _inGameController.SetJumpActive(true,PlayerStatsBlack.JumpsAllowed - StateContext.JumpCount);
        }
        
        _inGameController.SetSuit(!StateContext.IsBlack);
        _inGameController.SetAbility(StateContext.IsBlack);
    }

    private void FixedUpdate()
    {
        IsGrounded = StateContext.IsGrounded;
        _playerChecks.GroundCheck();
        _playerChecks.isTouchingWall();

        if (StateContext.IsGrounded)
        {
            StateContext.SideDashesUsed = 0;
            StateContext.DownDashesUsed = 0;
        }

        _stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {
        _stateMachine.LateUpdate();
    }
    private void UpdateTimers(float time)
    {
        foreach (var timer in _timers)
        {
            timer.Tick(time);
        }
    }


    /// <summary>
    /// Apply damage to the player and trigger damage flow (animation, knockback, invulnerability frames).
    /// </summary>
    public void TakeDamage(int damage,EnemyType type, GameObject enemyGameObject)
    {
        if (StateContext.IsDamaged)
            return;


        
        StateContext.CurrentHealth -= damage;
        _inGameController.SetHealth(StateContext.CurrentHealth / (float)_playerStatsBlack.MaxHealth);
        if (StateContext.CurrentHealth <= 0)
        {
            OnDeath();
            return;
        }

        // Play damaged animation and start coroutine to handle knockback and recovery
        int prevAnimHash = AnimationController.CurrentAnimationHash;
        AnimationController.ChangeAnimation(Animator.StringToHash("DAMAGED"), true);
        StartCoroutine(_TakeDamage(damage, type, enemyGameObject, prevAnimHash));
    }

    // Coroutine that handles the respawn sequence with particles, cutscenes and re-enabling controls
    private IEnumerator Respawn()
    {
        PlayerSounds.PlayDeathSound();
        StateContext.IsSwinging = false;
        StateContext.IsPulling = false;
        _input.DisablePlayerActions(); // Disable input while respawn sequence runs

        // Play the correct death particle effect based on current suit
        if (StateContext.IsBlack)
            _deathBlackParticles.Play();
        else
            _deathRedParticles.Play();

        // Hide player sprite during death/respawn visuals
        _spriteRenderer.enabled = false;

        // Special-case: when level index == 3, play a short cutscene then reload the level
        if (SettingsManager.globalSave.level == 3)
        {
            yield return new WaitForSeconds(0.2f);
            _deathCutScene.Play();
            yield return new WaitForSeconds(0.5f);

            _spriteRenderer.enabled = true;

            // Clear any active camera impulses and reload the scene
            CinemachineImpulseManager.Instance.Clear();
            SceneManager.LoadScene("3level");
            yield return null;
        }

        // Ensure idle animation is set before continuing
        if (StateContext.IsBlack)
            AnimationController.ChangeAnimation(StateContext.IdleAnimationHash);
        else
            AnimationController.ChangeAnimation(StateContext.IdleRedAnimationHash);

        StateContext.IsDamaged = true;

        yield return new WaitForSeconds(0.2f);

        // Play death cutscene and wait briefly
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);

        // Reset cutscenes so they can be evaluated precisely on respawn
        _deathCutScene.Stop();
        _deathCutScene.time = 0f;
        _deathCutScene.Evaluate();
        _respawnCutScene.time = 0f;
        _respawnCutScene.Evaluate();

        // Move player to checkpoint and update HUD health
        _levelSetupObject.Respawn();
        _inGameController.SetHealth(StateContext.CurrentHealth / (float)_playerStatsBlack.MaxHealth);

        yield return new WaitForSeconds(0.35f);

        _spriteRenderer.enabled = true;

        // Play respawn cutscene then restore control
        _respawnCutScene.Play();
        yield return new WaitForSeconds(0.5f);

        StateContext.IsDamaged = false;
        _respawnCutScene.Stop();
        _input.EnablePlayerActions();
    }

    public void OnDeath()
    {
        StartCoroutine(Respawn());  

    }

    public void Heal(int healAmount)
    {
        StateContext.CurrentHealth += healAmount;
        if (StateContext.CurrentHealth > _playerStatsBlack.MaxHealth)
            StateContext.CurrentHealth = _playerStatsBlack.MaxHealth;
        _inGameController.SetHealth(StateContext.CurrentHealth / (float)_playerStatsBlack.MaxHealth);
    }

    // Internal coroutine that handles damage recovery, knockback application and temporary invulnerability
    private IEnumerator _TakeDamage(int damage, EnemyType type, GameObject enemyGameObject, int prevAnimHash)
    {
        StateContext.IsDamaged = true;
        StateContext.currentCombatId = 0;

        // Apply different knockback depending on enemy type (shielded enemies may have different force)
        if (type == EnemyType.SHIELD)
        {
            _rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - enemyGameObject.transform.position.x), 0.3f) * 2f * _playerStatsBlack.DamageShieldKnockbackForce, ForceMode2D.Impulse);
        }
        else if (type != EnemyType.TRAP)
        {
            _rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - enemyGameObject.transform.position.x), 0.3f) * 2f * _playerStatsBlack.DamageUsualKnockbackForce, ForceMode2D.Impulse);
        }

        // Play damage sound feedback
        PlayerSounds.PlayDamageSound();

        // Wait for the configured damage state duration, then reset state and restore previous animation
        yield return new WaitForSeconds(_playerStatsBlack.DamageStateTime);
        StateContext.currentCombatId = 0;
        StateContext.IsDamaged = false;
        AnimationController.ChangeAnimation(prevAnimHash);
    }

    // Draw attack radius and center in the editor for debugging purposes
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(
            new Vector3(
                transform.position.x + Mathf.Sign(transform.localScale.x) * _playerStatsBlack.AttackCenterRelativeToPlayer.x,
                transform.position.y + _playerStatsBlack.AttackCenterRelativeToPlayer.y,
                0f
            ),
            _playerStatsBlack.AttackRadius
        );
    }

    // Handle anchor (hook) detection while staying in trigger - manages nearest anchor activation/deactivation
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == _playerRedStats.AnchorTag)
        {
            if (StateContext.nearestAnchor != null)
            {
                // If this collision is further away than the current nearest anchor, preserve the nearest anchor
                if (Vector2.Distance(collision.gameObject.transform.position, transform.position) >=
                    Vector2.Distance(StateContext.nearestAnchor.transform.position, transform.position))
                {
                    if (!StateContext.IsBlack)
                        StateContext.nearestAnchor.GetComponent<Anchor>().Activate();
                    else
                        StateContext.nearestAnchor.GetComponent<Anchor>().Deactivate();
                    return;
                }

                // Deactivate previous nearest anchor when a closer one is found
                StateContext.nearestAnchor.GetComponent<Anchor>().Deactivate();
            }

            // Set this collision as the nearest anchor and activate/deactivate based on suit
            StateContext.nearestAnchor = collision.gameObject;
            if (!StateContext.IsBlack)
                StateContext.nearestAnchor.GetComponent<Anchor>().Activate();
            else
                StateContext.nearestAnchor.GetComponent<Anchor>().Deactivate();
        }
    }

    // Clean up nearest anchor when leaving its trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == _playerRedStats.AnchorTag)
        {
            if (StateContext.nearestAnchor != collision.gameObject)
                return;
            StateContext.nearestAnchor.GetComponent<Anchor>().Deactivate();
            StateContext.nearestAnchor = null;
        }
    }

    // Gradually interpolate a sprite renderer's color to a target and destroy the temporary sprite when finished
    private IEnumerator LerpColorCoroutine(float duration, SpriteRenderer sr, Color target)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float a = Mathf.Lerp(sr.color.a, target.a, elapsed / duration);
            float g = Mathf.Lerp(sr.color.g, target.g, elapsed / duration);
            float b = Mathf.Lerp(sr.color.b, target.b, elapsed / duration);
            float r = Mathf.Lerp(sr.color.r, target.r, elapsed / duration);

            sr.color = new Color(r, g, b, a);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(sr.gameObject);
    }

    // Create an after-image sprite for dash and fade it over time
    public void AfterPerformImage(GameObject sprite, Color startColor, Color targetColor)
    {
        GameObject go = Instantiate(sprite, this.transform.position, this.transform.rotation);
        SpriteRenderer img = go.GetComponent<SpriteRenderer>();

        img.color = startColor;
        go.transform.localScale = this.transform.localScale;

        // Start fade coroutine for the temporary after-image
        StartCoroutine(LerpColorCoroutine(0.8f, img, targetColor));
    }
}
