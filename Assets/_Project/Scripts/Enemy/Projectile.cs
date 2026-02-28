using UnityEngine;

public class Projectile : Bullet
{

    private Vector2 currentSpeed;
    private bool moving = false;
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

    private void FixedUpdate()
    {
        if (moving)
            _rb.linearVelocity = currentSpeed;
    }

    public void Shoot(Vector2 direction)
    {
        currentSpeed = direction * _speed;
        moving = true;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }
}
