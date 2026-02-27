using UnityEngine;

public class FSMStagger : FSMState
{
    private float staggerEndTime;
    private FSMState attackState;

    public FSMStagger(NPCController npc, float duration, FSMState attackState) : base(npc)
    {
        this.staggerEndTime = Time.time + duration;
        this.attackState = attackState;
    }

    public override void Enter()
    {
        // Stop movement
        Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Keep vertical velocity for physics
        } if (npc is NPCWalkingRange walkingRange) {
            walkingRange.gun.SetActive(true);
        }
    }

    public override void Update()
    {
        // Check if stagger duration has ended
        if (Time.time >= staggerEndTime) {
            // Return to previous state or idle/patrol
            if (attackState != null) {
                npc.ChangeState(attackState);
            }
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        if (npc is NPCWalkingRange walkingRange) {
            walkingRange.gun.SetActive(false);
        }
    }
    }