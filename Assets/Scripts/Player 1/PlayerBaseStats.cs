using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "PlayerBaseStats", menuName = "Scriptable Objects/PlayerBaseStats")]
public abstract class PlayerBaseStats : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] protected float _runSpeed;
    [SerializeField] protected float _airSpeed;
    [SerializeField] protected float _groundAcceleration;
    [SerializeField] protected float _groundDeceleration;
    [SerializeField] protected float _airAcceleration;
    [SerializeField] protected float _airDeceleration;
    [SerializeField] protected float _groundAccelerationPow;
    [SerializeField] protected float _airAccelerationPow;
    [SerializeField] protected LayerMask _playerLayer;

    [Header("Jump Settings")]
    [SerializeField] protected float _groundCheckDistance;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float jumpCutVelocity;
    [SerializeField] protected float _fallGravityMultiplier;
    [SerializeField] protected float _maxFallSpeed;
    [SerializeField, Range(0, 255)] protected int _jumpsAllowed = 2;
    [SerializeField] protected float _edgeDetectionDistance;
    [SerializeField] protected float _maxEdgeCorrectionDistance;
    [SerializeField] protected float _apexHangTime;
    [SerializeField] protected float _coyoteTime;
    [SerializeField] protected float _jumpBufferTime;

    [Header("Dash Settings")]
    [SerializeField] protected float _dashForce;
    [SerializeField] protected float _dashTime;

    [Header("Attack Settings")]
    [SerializeField] protected Vector2 _attackCenterRelativeToPlayer;
    [SerializeField] protected float _attackRadius;
    [SerializeField] protected LayerMask _enemyLayer;
    [SerializeField] protected int _attackDamage;
    [SerializeField] protected float _hitStopDuration;
    [SerializeField] protected float _combatBufferTime;


    [Header("Melee Attack Crit Settings")]
    [SerializeField] protected float _meleeCritMultiplier;
    [SerializeField] protected CameraShakeSettings _MeleeCritCameraShake;

    [Header("Animation Settings")]
    [SerializeField] protected string _idleAnimationName;
    [SerializeField] protected string _runAnimationName;
    [SerializeField] protected string _jumpAnimationName;
    [SerializeField] protected string _fallAnimationName;
    [SerializeField] protected string _combatAttack1AnimationName;
    [SerializeField] protected string _combatAttack2AnimationName;
    [SerializeField] protected string _combatAttack3AnimationName;
    [SerializeField] protected string _sideDashAnimationName;
    [SerializeField] protected string _glidingAnimationName;
    [SerializeField] protected string _glidingRevAnimationName;
    [SerializeField] protected string _changeSuit;
    [SerializeField] protected string _downDashAnimationName;
    [SerializeField] protected string _wallSlideAnimationName;


    [Header("Damaged Settings")]
    [SerializeField] protected float _damageInvincibilityTime;
    [SerializeField] protected float _damageStateTime;
    [SerializeField] protected float _damageShieldKnockbackForce;
    [SerializeField] protected float _damageUsualKnockbackForce;
    [SerializeField] protected int  _maxHealth;


    [Header("After image")]
    [SerializeField] protected GameObject _afterImageSideDashSprite;
    [SerializeField] protected GameObject _afterImageDownDashSprite;
    [SerializeField] protected GameObject _afterImageJumpSprite;
    [SerializeField] protected Color _afterImageColor;
    [SerializeField] protected Color _afterImageTargetColor;
    [SerializeField] protected float _timeBetweenAfterImages;
    [SerializeField] protected float _afterImageColorStep;


    public float AfterImageColorStep => _afterImageColorStep;
    public float TimeBetweenAfterImages => _timeBetweenAfterImages;
    public GameObject AfterImageSideDashSprite => _afterImageSideDashSprite;
    public GameObject AfterImageDownDashSprite => _afterImageDownDashSprite;
    public GameObject AfterImageJumpSprite => _afterImageJumpSprite;
    public Color AfterImageColor => _afterImageColor;
    public Color AfterImageTargetColor => _afterImageTargetColor;
    public string GlidingAnimationName => _glidingAnimationName;
    public int MaxHealth => _maxHealth;
    public string ChangeSuit => _changeSuit;
    public string GlidingRevAnimationName => _glidingRevAnimationName;
    public string DownDashAnimationName => _downDashAnimationName;
    public string WallSlideAnimationName => _wallSlideAnimationName;
    public string SideDashAnimationName => _sideDashAnimationName;
    public float DamageUsualKnockbackForce => _damageUsualKnockbackForce;
    public float DamageShieldKnockbackForce => _damageShieldKnockbackForce; 
    public float DamageInvincibilityTime => _damageInvincibilityTime;
    public float DamageStateTime => _damageStateTime;
    public float RunSpeed => _runSpeed;
    public float AirSpeed => _airSpeed;
    public float GroundAcceleration => _groundAcceleration;
    public float GroundDeceleration => _groundDeceleration;
    public float AirAcceleration => _airAcceleration;
    public float AirDeceleration => _airDeceleration;
    public float GroundAccelerationPow => _groundAccelerationPow;
    public float AirAccelerationPow => _airAccelerationPow;

    public float GroundCheckDistance => _groundCheckDistance;
    public LayerMask GroundLayer => _groundLayer;
    public float JumpForce => _jumpForce;
    public float JumpCutVelocity => jumpCutVelocity;
    public float FallGravityMultiplier => _fallGravityMultiplier;
    public float MaxFallSpeed => _maxFallSpeed;
    public int JumpsAllowed => _jumpsAllowed;
    public float EdgeDetectionDistance => _edgeDetectionDistance;
    public float MaxEdgeCorrectionDistance => _maxEdgeCorrectionDistance;
    public float ApexHangTime => _apexHangTime;
    public float CoyoteTime => _coyoteTime;
    public float JumpBufferTime => _jumpBufferTime;
    public float DashForce => _dashForce;
    public float DashTime => _dashTime;
    public string IdleAnimationName => _idleAnimationName;
    public string RunAnimationName => _runAnimationName;

    public string JumpAnimationName => _jumpAnimationName;
    public string FallAnimationName => _fallAnimationName;
    public string CombatAttack1AnimationName => _combatAttack1AnimationName;
    public string CombatAttack2AnimationName => _combatAttack2AnimationName;
    public string CombatAttack3AnimationName => _combatAttack3AnimationName;

    public Vector2 AttackCenterRelativeToPlayer => _attackCenterRelativeToPlayer;
    public float AttackRadius => _attackRadius;
    public LayerMask EnemyLayer => _enemyLayer;
    public int AttackDamage => _attackDamage;
    public float HitStopDuration => _hitStopDuration;
    public float CombatBufferTime => _combatBufferTime;
    public float MeleeCritMultiplier => _meleeCritMultiplier;

    public CameraShakeSettings MeleeCritCameraShake => _MeleeCritCameraShake;

}
