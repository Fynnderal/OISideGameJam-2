using UnityEngine;

// A state where the NPC waits for a specified delay before transitioning to the next state. IDLE state.
public class FSMWait : FSMState
{
    private float startTime;
    private float delay;

    public FSMWait(NPCController npc, float delay) : base(npc)
    {
        this.delay = delay;
    }

    public override void Enter()
    {
        startTime = Time.time;
    }

    public override void Update()
    {
        if (Time.time - startTime > delay)
        {
            npc.StopState();
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {

    }
}
