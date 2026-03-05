using KBCore.Refs;
using UnityEngine;

public class SinusProjectile : ValidatedMonoBehaviour
{

    [SerializeField] private float _turnSpeed;

    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;
    [SerializeField, Self] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 8f;
    [SerializeField] private int _damage = 30;



    private Vector2 _targetPosition;
    private Vector2 _targetDirection;
    private Vector2 _right;
    private float _time = 0f;
    private Vector2 _trajectoryPosition;



    private void Start()
    {
        _targetPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        _targetDirection = (_targetPosition - (Vector2)transform.position).normalized;
        _right = Vector2.Perpendicular(_targetDirection).normalized;

        _trajectoryPosition = _rb.position;
    }

    // Update is called once per frame


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
            return;

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<IDamagablle>()?.TakeDamage(_damage);
        }
        
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;



        float sinusOffset = Mathf.Sin(_time * _frequency) * _amplitude;
        Vector2 offset = _right * sinusOffset;

        _trajectoryPosition += _targetDirection * _speed;
        _rb.MovePosition(_trajectoryPosition + offset);


    }
}
