
using UnityEngine;

// Ranged NPC that stands still and shoots at players within range
public class NPCStandingRange : NPCRange
{

    [SerializeField] public GameObject gun;

    private void Start()
    { 
        detectionCollider.isTrigger = true;
        type = EnemyType.STATICRANGE;
        FSMIdle = new FSMWait(this, 1.0f); // Idle state with a brief wait
        ChangeState(FSMIdle); // Start with the idle state
    }

    protected override void Update()
    {

   
        base.Update();
    }

    public override bool is_wizard()
    {
        return false;
    }
}
