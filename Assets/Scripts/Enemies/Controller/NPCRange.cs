using Unity.VisualScripting;
using UnityEngine;

// Ranged NPC parent class
public abstract class NPCRange : NPCController, IEnemy, IEnemyDynamic
{

    public FSMState FSMIdle;
    public GameObject bullet;


    [SerializeField] public int health = 100;
    [SerializeField] public float speed = 1.5f;
    [SerializeField] public float acceleration = 15f;
    [SerializeField] public float bulletAcceleration = 0f; // Not used currently
    [SerializeField] public float selfKnockback = 2f;
    [SerializeField] public float atkSpeed = 1.0f;
    [SerializeField] public float bulletSpeed = 10f;
    [SerializeField] public int damage = 20;
    [SerializeField] public int onKillHealthReward = 20;

    [SerializeField] public Suit suit = Suit.RED;

    protected Sprite sprite;
    [SerializeField] public SpriteRenderer spriteRenderer;

    [SerializeField] public ParticleSystem bloodParticle;
    [SerializeField] public GameObject shootParticle;
    [SerializeField] public GameObject deathParticle;

    [SerializeField] public CircleCollider2D detectionCollider;

    [SerializeField] public GameObject attackIndicatorPrefab;

    [SerializeField] public Animator animator;

    [SerializeField] public float staggerDuration = 0.5f;

    [SerializeField] protected EnemySounds enemySounds;

    
    protected Collider2D ground;


    private bool isDead = false;


    protected override void Update()
    {
        base.Update();
    }

    // Just a helper to see the state visually
    protected void SetColor(Color color)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = color;
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
        if (isDead) return false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (staggerDuration > 0) ChangeState(new FSMStagger(this, staggerDuration, new FSMShooting(this, player.GetComponent<PlayerController>())));

        int actualDamage = (suit == this.suit) ? 2 * damage : damage;
        health -= actualDamage;
        rb.AddForce(new Vector2(Mathf.Sign(transform.position.x - player.transform.position.x), 1f) * actualDamage * selfKnockback);

        bloodParticle.Play();
   
        
        if (health <= 0) {
            Die();
            // Get PlayerController first, then access StateContext property
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.StateContext != null)
            {
                //Debug.Log("Current Health before reward: " + playerController.StateContext.CurrentHealth);
                playerController.Heal(onKillHealthReward);
                //Debug.Log("Current Health after reward: " + playerController.StateContext.CurrentHealth);
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
        Debug.Log($"{name} has died!");

        Animator animator = GetComponent<Animator>();
        if (animator != null) {
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


    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            if (currentState is FSMShooting) return;
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                //Debug.Log($"Player {player.name} entered chase range.");
                animator.SetBool("isShooting", true);
                if(enemySounds != null) enemySounds.PlayPlayerDetectedSound();

                ChangeState(new FSMShooting(this, player));
                
            }
        }
    }

    // Detect player leaving trigger
    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                //Debug.Log($"Player {player.name} left chase range.");
                animator.SetBool("isShooting", false);
                ChangeState(new FSMPatrol(this, animator));
            }
        }
    }

    protected int direction = 1;
    public void Flip()
    {
        if (isDead) return;
        direction = -direction;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.flipX = !sr.flipX;
        }
    }


    /// <summary>
    /// Gets the ground collider directly below the NPC
    /// </summary>
    protected Collider2D GetGround(float maxDistance = 1f)
    {
        if (ground != null) {
            if (Vector2.Distance(ground.transform.position, transform.position) < 5f &&
                transform.position.y - ground.transform.position.y < 3 &&
                transform.position.y - ground.transform.position.y > 0) return ground;
        }

        int groundLayer = LayerMask.GetMask("Ground");
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, maxDistance, groundLayer);

        Collider2D closest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits) {
            if (hit.collider != null) {
                float distance = hit.distance;
                if (distance < minDistance) {
                    minDistance = distance;
                    closest = hit.collider;
                }
            }
        }
        ground = closest;
        return closest;
    }

    public EnemySounds GetEnemySounds()
    {
        return enemySounds;
    }

    public abstract bool is_wizard();
}
