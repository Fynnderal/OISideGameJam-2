using UnityEngine;
using System.Collections;

public class NPCClassic : NPCController, IEnemy, IEnemyDynamic
{

    [SerializeField] public int health = 100;
    [SerializeField] public float speed = 4f;
    [SerializeField] public float acceleration = 20f;
    [SerializeField] public float selfKnockback = 2f;
    [SerializeField] public int onKillHealthReward = 20;

    [SerializeField] public Suit suit = Suit.RED;


    protected bool isDead = false;

    [SerializeField] public BoxCollider2D view = null;
    [SerializeField] public BoxCollider2D collision = null;

    [SerializeField] protected ParticleSystem bloodParticle;
    [SerializeField] public GameObject deathParticle;

    [SerializeField] public Animator animator;

    [SerializeField] public float staggerDuration = 0.3f;

    [SerializeField] protected EnemySounds enemySounds;


    // Believe better optimized player finding using triggers.
    // Detect player entering trigger
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                //Debug.Log($"Player {player.name} entered chase range.");
                ChangeState(new FSMChase(this, player));
                //GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    // Detect player leaving trigger
    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                //Debug.Log($"Player {player.name} left chase range.");
                ChangeState(new FSMPatrol(this, animator));
                //GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    //IEnemyDynamic
    public float GetSpeed()
    {
        return speed;
    }
    public float GetAcceleration()
    {
        return acceleration;
    }

    /// <summary>
    /// Handles taking damage, applying knockback, playing effects, and dying.
    /// </summary>
    public virtual bool TakeDamage(int damage, Suit suit, GameObject player)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (staggerDuration > 0) ChangeState(new FSMStagger(this, staggerDuration, new FSMChase(this, player.GetComponent<PlayerController>())));

        int actualDamage = (suit == this.suit) ? 2 * damage : damage;
        health -= actualDamage;
        rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - player.transform.position.x), 1f) * actualDamage * selfKnockback);

        //Debug.Log("Enemy " + this.name + "taken damage");
        bloodParticle.Play();

        if (health <= 0) {
            Die();
            // Get PlayerController first, then access StateContext property
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.StateContext != null) {
                playerController.Heal(onKillHealthReward);
            }
        } else {
            if (enemySounds != null) enemySounds.PlayDamageSound(actualDamage);
        }

        var flash = GetComponent<SpriteFlashController>();
        if (flash != null) flash.FlashOnce();
        return true;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        //Debug.Log($"{name} has died!");

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (enemySounds != null) enemySounds.PlayDeathSound();
        Destroy(gameObject, 0.05f);
        if (deathParticle != null) {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
    }


    protected Collider2D ground;

    /// <summary>
    /// Gets the ground collider directly below the NPC
    /// </summary>
    protected Collider2D GetGround(float maxDistance = 1f)
    {
        if (ground != null)
        {
            if (Vector2.Distance(ground.transform.position, transform.position) < 5f &&
                transform.position.y - ground.transform.position.y < 3 &&
                transform.position.y - ground.transform.position.y > 0) return ground;
        }

        int groundLayer = LayerMask.GetMask("Ground");

        Vector2 origin = new Vector2(collision.bounds.center.x, collision.bounds.center.y);
        Vector2 size = new Vector2(collision.bounds.size.x, collision.bounds.size.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, size, 0f, Vector2.down, maxDistance, groundLayer);

        Collider2D closest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                float distance = hit.distance;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = hit.collider;
                }
            }
        }
        ground = closest;
        return closest;
    }


    protected int direction = 1;
    public void Flip()
    {
        direction = -direction;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.flipX = !sr.flipX;
        }

    }

    public EnemySounds GetEnemySounds()
    {
        return enemySounds;
    }

}

