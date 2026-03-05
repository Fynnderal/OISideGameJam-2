using KBCore.Refs;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

public class EnemyBase : ValidatedMonoBehaviour, IDamagablle
{
    [SerializeField] protected float _speed = 2f;
    [SerializeField, Self] protected Rigidbody2D _rb;
    [SerializeField] protected float _attackRadius = 1f;
    [SerializeField] protected float _takenDamageMultiplier = 1f;
    [SerializeField] protected float _health = 100;
    [SerializeField, Self] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected float separationRadius = 0.3f;
    [SerializeField] protected float separationStrength = 0.3f;
    [SerializeField] protected float separationRayCastDistance = 4.5f;
    [SerializeField] protected float raycastSeparationStrength = 1.5f;
    [SerializeField, Self] protected Animator _animator;
    [SerializeField] protected string _deathAnimationName = "Death";
    [SerializeField] protected string _damageAnimationName = "Damage";
    [SerializeField] protected string _idleAnimationName = "Idle";
    [SerializeField] protected string _runAnimationName = "Run";
    [SerializeField] protected string _attackAnimationName = "Attack";
    [SerializeField] protected float _damageAnimationDuration = 0.1f;
    [SerializeField] protected float _attackCooldown = 3f;
    [SerializeField] protected float _attackDuration = 0.5f;
    [SerializeField, Self] protected Collider2D _collider;



    protected Transform _target;
    protected Vector2 _moveDirection;
    protected Vector2 _distance;
    protected Vector2 _separation;
    protected AnimationController _animationController;


    protected int _deathAnimationHash;
    protected int _idleAnimationHash;
    protected int _runAnimationHash;
    protected int _attackAnimationHash;
    protected int _damageAnimationHash;


    protected bool _Damaged = false;
    protected bool _isDead = false;
    public bool IsOnFire { get; set; }

    protected bool isAttacking = false;
    protected CountdownTimer _attackCooldownTimer;
    protected CountdownTimer _attackDurationTimer;
    protected CountdownTimer _wakeUpTimer;
    virtual protected void Start()
    {
        _attackCooldownTimer = new CountdownTimer(_attackCooldown);
        _attackDurationTimer = new CountdownTimer(_attackDuration);
        _wakeUpTimer = new CountdownTimer(2f);
        _attackDurationTimer.OnTimerStop += () => isAttacking = false;

        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _animationController = new AnimationController(_animator);

        _deathAnimationHash = Animator.StringToHash(_deathAnimationName);
        _idleAnimationHash = Animator.StringToHash(_idleAnimationName);
        _runAnimationHash = Animator.StringToHash(_runAnimationName);
        _attackAnimationHash = Animator.StringToHash(_attackAnimationName);
        _damageAnimationHash = Animator.StringToHash(_damageAnimationName);

        _wakeUpTimer.Start();

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(_rb.position, separationRadius);
    //}
    void AntiCrowd() {
        _separation = Vector2.zero;

        RaycastHit2D ray_hit = Physics2D.Raycast(_rb.position, _moveDirection, separationRayCastDistance, enemyLayer);

        if (ray_hit)
        {
            _separation = Vector2.Perpendicular(_moveDirection).normalized;
            return;
        }



        //Collider2D[] hits = Physics2D.OverlapCircleAll(
        //    _rb.position,
        //    separationRadius,
        //    enemyLayer
        //);

        //int count = 0;

        //foreach (var hit in hits)
        //{
        //    if (hit.attachedRigidbody == _rb)
        //        continue;

        //    Vector2 diff = _rb.position - hit.attachedRigidbody.position;
        //    float distance = diff.magnitude;


        //    _separation += distance == 0 ? diff.normalized * 10f : diff.normalized / distance;

        //}

        //if (count > 0)
        //{
        //    _separation /= count;
        //}



    }

    virtual protected void Update()
    {
        _attackCooldownTimer.Tick(Time.deltaTime);
        _attackDurationTimer.Tick(Time.deltaTime);
        _wakeUpTimer.Tick(Time.deltaTime);



        if (_isDead || _attackDurationTimer.isRunning || _wakeUpTimer.isRunning)
            return;


        _distance = (_target.position - transform.position);
        _moveDirection = _distance.normalized;

        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;

        Vector3 localScale = transform.localScale;
        
        if (angle < 90 && angle > -90)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);

        transform.localScale = localScale;

        if (_distance.magnitude <= _attackRadius)
        {
            Attack();
        }





    }

    virtual protected void FixedUpdate()
    {
        if (_isDead || _attackDurationTimer.isRunning || _wakeUpTimer.isRunning)
            return;
        
        AntiCrowd();

        if (_distance.magnitude > _attackRadius)
        {
            _rb.linearVelocity = (_moveDirection.normalized + _separation * separationStrength) * _speed;
            if (!_Damaged && !_attackDurationTimer.isRunning)
            {
                _animationController.ChangeAnimation(_runAnimationHash);
            }
        }
        else
        {
            if (!_Damaged && !_attackDurationTimer.isRunning)
            {
 
                _animationController.ChangeAnimation(_idleAnimationHash);
            }
            _rb.linearVelocity = Vector2.zero;
        }
    }

    virtual protected void Attack()
    {
        if (_Damaged || _attackCooldownTimer.isRunning)
            return;

        isAttacking = true;
        _animationController.ChangeAnimation(_attackAnimationHash);
        _rb.linearVelocity = Vector2.zero;
        _attackDurationTimer.Start();
        _attackCooldownTimer.Start();


    }

    virtual public void PerformAttack()
    {
        
    }

    virtual public void TakeDamage(float damage)
    {
        if (_Damaged || _isDead)
            return;

        _health -= (damage * _takenDamageMultiplier);
        if (_health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(TakeDamageCoroutine());
        }

    }

    protected IEnumerator TakeDamageCoroutine()
    {
        _Damaged = true;
        _animationController.ChangeAnimation(_damageAnimationHash, true);
        yield return new WaitForSeconds(0.05f);
        _Damaged = false;
    }

    virtual protected void Die()
    {
        _rb.linearVelocity = Vector2.zero;
        _animationController.ChangeAnimation(_deathAnimationHash);
        
        _collider.enabled = false;

        _isDead = true;
        Destroy(gameObject, 2f);
    }
}
