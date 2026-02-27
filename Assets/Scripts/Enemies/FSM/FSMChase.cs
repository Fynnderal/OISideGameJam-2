using UnityEngine;

public class FSMChase : FSMState
{
    public PlayerController pc;
    private IEnemyAttack attacker;
    private IEnemyDynamic dynamic;
    private Rigidbody2D rb;
    private NPCClassic npcc;
    private Collider2D collision;

    private float attackCooldown = 0f;

    private string isAttackingParam = "isAttacking";
    
    // Track if we were in attack range last frame
    private bool wasInAttackRange = false;
    
    // Cache the melee reference (null if not NPCMelee)
    private NPCMelee meleeNpc;

    public FSMChase(NPCClassic npc, PlayerController pc) : base(npc)
    {
        this.pc = pc;
        this.npcc = npc;
        rb = npcc.GetComponent<Rigidbody2D>();
        collision = npcc.collision;
        attacker = npcc as IEnemyAttack;
        if (attacker == null)
        {
            Debug.LogWarning("NPC does not implement IEnemyAttack!");
        }
        dynamic = npcc as IEnemyDynamic;
        if (dynamic == null)
        {
            Debug.LogWarning("NPC does not implement IEnemyDynamic!");
        }
        
        // Cache melee reference for attack cancellation
        meleeNpc = npcc as NPCMelee;
    }

    public override void Enter()
    {
        npcc.animator.SetFloat("Speed", 2f);
        npcc.animator.SetBool("isWalking", true);
        wasInAttackRange = false;
    }

    /// <summary>
    /// Main chase logic - chases player and attacks when in range.
    /// Handles movement, attack cooldowns, edge detection, and sprite flipping.
    /// </summary>
    public override void Update()
    {
        if (pc == null) return;

        // Tick down attack cooldown
        if (attackCooldown > 0f)
            attackCooldown -= Time.deltaTime;

        // Calculate horizontal direction and distance to player
        Vector2 direction = pc.transform.position - npcc.transform.position;
        direction.y = 0;
        float distanceX = Mathf.Abs(direction.x);
        float distance = Vector2.Distance(npcc.transform.position, pc.transform.position);

        float attackRange = attacker.GetAttackRange();
        bool isInAttackRange = distanceX <= attackRange;

        // NPCMelee only: cancel attack combo if player escaped range mid-attack
        if (meleeNpc != null && wasInAttackRange && !isInAttackRange)
        {
            if (meleeNpc.IsAttacking())
            {
                meleeNpc.CancelAttack();
                attackCooldown = 0f; // Allow immediate re-attack when catching up
            }
        }
        wasInAttackRange = isInAttackRange;

        // Move toward player if not in attack range
        float targetSpeed = 0f;
        if (!isInAttackRange)
        {
            npcc.animator.SetBool("isWalking", true);
            npcc.GetEnemySounds().UpdateWalkingSound(true, 1.5f);
            targetSpeed = Mathf.Sign(direction.x) * dynamic.GetSpeed();
        } else {
            npcc.animator.SetBool("isWalking", false);
        }

        // stop if no ground ahead to prevent falling off platforms
        float groundCheckDistance = (npcc.transform.localScale.y) + 0.1f;
        Vector2 forward = new Vector2(Mathf.Sign(direction.x), 0);

        // Cast from the front edge of the collider
        Vector2 origin;
        if (forward.x > 0)
            origin = new Vector2(collision.bounds.max.x, collision.bounds.center.y);
        else
            origin = new Vector2(collision.bounds.min.x, collision.bounds.center.y);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);

        // No ground ahead - stop to avoid falling
        if (hit.collider == null)
        {
            targetSpeed = 0;
        }

        // Smoothly accelerate/decelerate toward target speed
        float velocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, dynamic.GetAcceleration() * Time.deltaTime);
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);

        // Flip sprite to face player (each NPC type handles flipping differently)
        if (direction.x != 0)
        {
            SpriteRenderer spriteRenderer = npcc.GetComponent<SpriteRenderer>();
            if (npcc is NPCMelee) spriteRenderer.flipX = direction.x > 0;
            else if (npcc is NPCShield) ((NPCShield)npcc).AutoFlip();
        }

        // Attack if in range and cooldown expired
        if (isInAttackRange && attackCooldown <= 0f) {
            npcc.animator.SetBool(isAttackingParam, true);
            npcc.animator.SetBool("isWalking", false);
            attacker.Attack(pc);
            attackCooldown = attacker.GetAttackDelay();
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        npcc.animator.SetFloat("Speed", 1f);
        
        // Cancel any ongoing attack when leaving chase state (NPCMelee only)
        if (meleeNpc != null)
        {
            meleeNpc.CancelAttack();
        }
    }
}

