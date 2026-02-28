using UnityEngine;
using KBCore.Refs;
using Utilities;
public class Bullet : ValidatedMonoBehaviour
{
    [SerializeField, Self] protected Rigidbody2D _rb;
    [SerializeField] protected float _speed = 15f;
    [SerializeField] protected float _lifeTime = 5f;
    [SerializeField] protected int _damage = 30;

    public float Speed => _speed;

    public Vector2 Direction { get; set; }

    Timer _lifeTimer;

    private GameObject _lastCollision;

    private void Start()
    {
        _lifeTimer = new CountdownTimer(_lifeTime);
        _lifeTimer.Start();
    }


    private void Update()
    {
        _lifeTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (_rb.linearVelocity.magnitude < Speed)
            Shoot();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_lastCollision == collision.gameObject)
            return;


        _lastCollision = collision.gameObject;


        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<IDamagablle>()?.TakeDamage(_damage);
        }



        if (!_lifeTimer.isRunning || collision.gameObject.tag == "Player")
            Destroy(gameObject);


        ContactPoint2D firstContact = collision.contacts[0];

        Direction = Vector2.Reflect(Direction, firstContact.normal);
        Shoot();
    }

    public void Shoot()
    {
        _rb.linearVelocity = Direction * _speed;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg);
    }
}
