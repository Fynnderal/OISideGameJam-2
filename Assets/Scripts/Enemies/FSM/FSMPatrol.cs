using UnityEngine;

public class FSMPatrol : FSMState
{
    private Collider2D groundCollider;
    private float delay;
    private float phaseTime;
    private Vector2 targetPosition;
    private bool moving = false;

    private Rigidbody2D rb;
    private Collider2D collision;
    private IEnemyDynamic dynamic;
    private float speedMultiplier = 0f;
    private string isWalkingParam = "isWalking";

    private Animator animator;

    // Wizard floating effect variables
    private bool isWizard = false;
    private BoxCollider2D boxCollider;
    private float floatSpeed = 2f;
    private float floatAmplitude = 0.15f;
    private float floatTime = 0f;
    private float baseColliderOffsetY = 0f;

    public FSMPatrol(NPCController npc, Animator animator, float delay = 2.0f, float speedMultiplier = 0.5f) : base(npc)
    {
        this.delay = delay;
        this.speedMultiplier = speedMultiplier;
        dynamic = npc as IEnemyDynamic;
        this.animator = animator;
        animator.SetBool(isWalkingParam, false);
        //Debug.Log("Patrol active");
        if (dynamic == null)
        {
            Debug.LogWarning("NPC does not implement IEnemyDynamic!");
        }

        rb = npc.GetComponent<Rigidbody2D>();
        collision = npc.GetComponent<Collider2D>();
        if (rb == null)
        {
            Debug.LogWarning("NPC does not have Rigidbody2D!");
        }

        // Check if this is a wizard
        isWizard = npc is NPCWizard;
        if (isWizard)
        {
            boxCollider = npc.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                baseColliderOffsetY = boxCollider.offset.y;
            }
        }
    }

    public override void Enter()
    {
        groundCollider = GetGround();
        targetPosition = npc.transform.position;
        moving = false;
        phaseTime = Time.time;
        floatTime = Time.time;
    }

    /// <summary>
    /// Main patrol logic - alternates between idle and moving phases.
    /// Idle: waits for delay, then picks random X position on current ground platform.
    /// Moving: walks toward target, stops when reached or timeout.
    /// </summary>
    public override void Update()
    {
        if (groundCollider == null)
        {
            groundCollider = GetGround();
            if (groundCollider == null) return;
            //Debug.Log("Patrol got ground");
        }

        // Pick new target after delay
        if (!moving && Time.time - phaseTime > delay)
        {
            //Debug.Log("Patrol moving");
            float x = Random.Range(groundCollider.bounds.min.x, groundCollider.bounds.max.x);
            float y = groundCollider.bounds.max.y + npc.GetComponent<Collider2D>().bounds.extents.y;
            targetPosition = new Vector2(x, y);
            moving = true;
            phaseTime = Time.time;

            if (animator != null) {
                animator.SetBool(isWalkingParam, true);
                //Debug.Log("animator param set to: " + animator.GetBool(isWalkingParam));
            }
        }

        Vector2 direction = targetPosition - (Vector2)npc.transform.position;
        direction.y = 0;
        float distance = direction.magnitude;
        float stoppingDistance = 0.05f;
        //Debug.Log($"Moving={moving}, direction={direction}, distance={distance}, targetPos={targetPosition}, pos={npc.transform.position}");

        if (moving)
        {
            // Play walking sounds
            if (npc is NPCRange range) {
                EnemySounds enemySounds = range.GetEnemySounds();
                if (enemySounds != null) {
                    enemySounds.UpdateWalkingSound(true, 1f);
                }
            }
            else if (npc is NPCClassic classic) {
                EnemySounds enemySounds = classic.GetEnemySounds();
                if (enemySounds != null) {
                    enemySounds.UpdateWalkingSound(true, 1f);
                }
            }

            // Stop if reached target or exceeded max move time
            if (distance <= stoppingDistance || Time.time - phaseTime > delay * 2)
            {
                moving = false;
                phaseTime = Time.time;

                // Smoothly slow down to zero
                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, new Vector2(0, rb.linearVelocity.y), dynamic.GetAcceleration() * Time.deltaTime);
                if (animator != null) {
                    animator.SetBool(isWalkingParam, false);
                    //Debug.Log("animator param set to: " + animator.GetBool(isWalkingParam));
                }
                return;
            }

            // Smoothly accelerate toward target speed
            direction.Normalize();
            float targetSpeed = direction.x * dynamic.GetSpeed() * 0.5f;
            float velocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, dynamic.GetAcceleration() * Time.deltaTime);
            rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);

            // Flip sprite based on movement direction (each NPC type handles this differently)
            if (direction.x != 0)
            {
                SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
                if (npc is NPCMelee) spriteRenderer.flipX = direction.x > 0;
                else if (npc is NPCShield) ((NPCShield)npc).AutoFlip();
                else if (npc is NPCWalkingRange) {
                    ((NPCWalkingRange)npc).spriteRenderer.flipX = direction.x > 0;
                }
            }

            // Apply wizard floating effect when moving
            if (isWizard)
            {
                UpdateWizardFloat();
            }
        }
        else
        {
            // Idle phase - decelerate to stop
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, new Vector2(0, rb.linearVelocity.y), dynamic.GetAcceleration() * Time.deltaTime);
            
            // Apply wizard floating effect when idle
            if (isWizard)
            {
                UpdateWizardFloat();
            }
        }
    }

    /// <summary>
    /// Creates a floating effect for wizards by changing the collider's Y offset.
    /// Uses sine wave for smooth up/down motion.
    /// </summary>
    private void UpdateWizardFloat()
    {
        if (boxCollider == null) return;

        // Calculate floating offset using sine wave
        float floatOffset = Mathf.Sin((Time.time - floatTime) * floatSpeed) * floatAmplitude;
        
        // Smoothly adjust the box collider's Y offset
        Vector2 newOffset = boxCollider.offset;
        newOffset.y = baseColliderOffsetY + floatOffset;
        boxCollider.offset = newOffset;
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        rb.linearVelocity = Vector2.zero;
        
        // Reset wizard collider offset to base value
        if (isWizard && boxCollider != null)
        {
            Vector2 resetOffset = boxCollider.offset;
            resetOffset.y = baseColliderOffsetY;
            boxCollider.offset = resetOffset;
        }
        
        Debug.Log("Patrol inactive");
    }

    /// <summary>
    /// Finds the ground platform directly below the NPC
    /// Returns the closest ground collider within maxDistance.
    /// Used to make sure that patrol movement is within platform bounds.
    /// </summary>
    private Collider2D GetGround(float maxDistance = 1f)
    {
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
        return closest;
    }
}
