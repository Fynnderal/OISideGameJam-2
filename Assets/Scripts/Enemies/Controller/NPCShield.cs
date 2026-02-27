using UnityEngine;
using System.Collections;

public class NPCShield : NPCClassic, IEnemyAttack
{

    [SerializeField] public float attackRange = 4f;
    [SerializeField] public float hitRange = 0.5f;
    [SerializeField] public float attackDelay = 1.5f;
    [SerializeField] public float attackWaitTime = 0.4f;
    [SerializeField] public int damage = 30;
    [SerializeField] public float dashLength = 0.8f;
    [SerializeField] public float dashResetDelay = 1.5f;
    [SerializeField] public float dashSpeed = 6f;
    [SerializeField] public float prepareTime = 0.4f;

    [SerializeField] private ParticleSystem shieldParticle;

    [SerializeField] public GameObject attackIndicatorPrefab;



    private bool isDashing = false;


    private void Start()
    {
        type = EnemyType.SHIELD;

        ChangeState(new FSMPatrol(this, animator));
    }

    protected override void Update()
    {

        base.Update();
    }

    /// <summary>
    /// Handles sprite flipping based on current state.
    /// direction uses inverted sign convention - flip when desiredDirection equals direction.
    /// </summary>
    public void AutoFlip()
    {
        if (currentState is FSMChase chase)
        {
            PlayerController pc = chase.pc;
            if (pc != null)
            {
                int desiredDirection = (int)Mathf.Sign(pc.transform.position.x - transform.position.x);

                if (!this.isDashing && desiredDirection == direction)
                {
                    Flip();
                }
            }
        }
        if (currentState is FSMPatrol patrol)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
            {
                int desiredDirection = rb.linearVelocity.x > 0 ? 1 : -1;

                if (desiredDirection == direction)
                {
                    Flip();
                }
            }
        }
    }

    //IEnemyAttack
    public void Attack(PlayerController player)
    {
        if (!isDead)
        {
            GameObject indicator = Instantiate(attackIndicatorPrefab, transform.position, transform.rotation);
            AttackIndicator ai = indicator.GetComponent<AttackIndicator>();
            ai.Init(attackWaitTime);
            
            StartCoroutine(PerformDashStrike(player));
        }
    }

    /// <summary>
    /// Executes the shield enemy's dash attack in phases:
    /// 1. Prepare
    /// 2. Dash
    /// 3. Hit check
    /// 4. Recovery
    /// </summary>
    private IEnumerator PerformDashStrike(PlayerController player)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogWarning("Shield enemy has no Rigidbody2D!");
            yield break;
        }

        // Prepare for dash - stop and lock direction toward player
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("isPreparing");
        enemySounds.PlayPlayerDetectedSound();
        float dir = Mathf.Sign(player.transform.position.x - transform.position.x);

        yield return new WaitForSeconds(prepareTime); // Wind-up delay
        
        // Flip to face dash direction (direction uses opposite sign convention)
        int desiredDirection = (int)dir;
        if (desiredDirection != -direction)
        {
            Flip();
        }
        this.isDashing = true;
        float startTime = Time.time;

        float groundCheckDistance = (collision.size.y * 0.5f) + 0.1f;
        float forwardOffset = 0.3f;

        // Dash loop - continues until dashLength expires, hits player, or reaches edge
        while (Time.time - startTime < dashLength)
        {
            // raycast from front edge of collider
            Vector2 forward = new Vector2(dir, 0);
            Vector2 origin;
            if (dir > 0)
                origin = new Vector2 (collision.bounds.max.x, collision.bounds.center.y);
            else 
                origin = new Vector2 (collision.bounds.min.x, collision.bounds.center.y);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
            Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);
            
            // Stop dash if no ground ahead (prevents falling off platforms)
            if (hit.collider == null)
            {
                break;
            }
            else
            {
                enemySounds.UpdateWalkingSound(true, 0.8f);
                rb.linearVelocity = new Vector2(dir * dashSpeed, rb.linearVelocity.y);
            }

            // Check for hit during dash
            if (Vector2.Distance(player.transform.position, transform.position) <= hitRange)
            {
                enemySounds.PlayAttackSound();
                player.TakeDamage(damage, type, gameObject);
                break;
            }

            yield return null;
        }

        // Recovery phase - stop and wait before next action
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isAttacking", false);
        animator.ResetTrigger("isPreparing");
        yield return new WaitForSeconds(dashResetDelay);
        this.isDashing = false;
    }

    /// <summary>
    /// Shield blocks attacks from the front.
    /// Compares player position to shield facing direction:
    /// Suit.NONE bypasses shield (environmental/trap damage).
    /// </summary>
    public override bool TakeDamage(int damage, Suit suit, GameObject player)
    {
        // Environmental damage bypasses shield
        if (suit == Suit.NONE)
            return base.TakeDamage(damage, suit, player);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // Check if attack is from the front (shielded side)
        if (Mathf.Sign(transform.position.x - player.transform.position.x) == direction)
        {
            shieldParticle.Play();
            enemySounds.PlaySpecialSound();
            return false; // Damage blocked
        }
        else
        {
            return base.TakeDamage(damage, suit, player); // Hit from behind
        }
    }


    public float GetAttackRange()
    {
        return attackRange;
    }
    public float GetAttackDelay()
    {
        return attackDelay;
    }
}
