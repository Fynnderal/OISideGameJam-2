using UnityEngine;
using KBCore.Refs;
using UnityEngine.EventSystems;

public class EnemyBase : ValidatedMonoBehaviour, IDamagablle
{
    [SerializeField] protected float _speed = 2f;
    [SerializeField, Self] protected Rigidbody2D _rb;
    [SerializeField] protected float _attackRadius = 1f;
    [SerializeField] protected float _takenDamageMultiplier = 1f;
    [SerializeField] protected int _health = 100;   


    protected Transform _target;
    protected Vector2 _moveDirection;
    protected Vector2 _distance;


    virtual protected void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    virtual protected void Update()
    {
        _distance = (_target.position - transform.position);
        _moveDirection = _distance.normalized;



    }

    virtual protected void FixedUpdate()
    {
        if (_distance.magnitude > _attackRadius)
            _rb.linearVelocity = _moveDirection * _speed;
        else
            _rb.linearVelocity = Vector2.zero;
    }

    virtual protected void Attack()
    {
        Debug.Log("Not implemented");   
    }

    virtual public void TakeDamage(int damage)
    {
        _health -= (int)(damage * _takenDamageMultiplier);
        if (_health <= 0)
        {
            Die();
        }

    }

    virtual protected void Die()
    {
        Destroy(gameObject);
    }
}
