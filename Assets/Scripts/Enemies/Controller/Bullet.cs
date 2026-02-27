using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{
    
    [SerializeField] BoxCollider2D Collider;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] ParticleSystem _bulletDestroyParticle;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Light2D bulletLight;
    private float speed;
    private Vector2 direction;
    private float lifeTime = 5.0f;
    private int damage = 20;
    private EnemyType EnemyRangeType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles the collision with other objects.
    /// </summary>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trigger") || collision.CompareTag("Anchor"))
            return;

        if (collision.CompareTag("Player"))
        {
            //Debug.Log("Bullet hit player");
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.TakeDamage(damage, EnemyRangeType, gameObject);
        }
        Collider.enabled = false;
        spriteRenderer.enabled = false;
        bulletLight.enabled = false;
        _bulletDestroyParticle.Play();
        Destroy(gameObject, 2f);
    }


    public void setSpeed(float spd, Vector2 dir)
    {
        speed = spd;
        direction = dir.normalized;
        //Debug.Log("Setting bullet speed to: " + speed + " in direction: " + direction.ToString() + "with rb : " + rb.ToString());
        rb.linearVelocity = direction * speed;
    }

    public void setUp(Vector2 dir, float spd, int damage, EnemyType enemyType)
    {

        setSpeed(spd, dir);
        //Debug.Log("Bullet setUp called and RB is : " + rb.linearVelocity.ToString());
        EnemyRangeType = enemyType;
        this.damage = damage;
    }
}
