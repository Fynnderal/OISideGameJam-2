using UnityEngine;

public class NPCWizard : NPCRange
{

    private void Start()
    {
        detectionCollider.isTrigger = true;
        type = EnemyType.MAGE;
        FSMIdle = new FSMPatrol(this, animator);
        ChangeState(FSMIdle); // Start with the idle state
    }

    protected override void Update()
    {
        // Visual feedback
        if (currentState is not FSMShooting) {

        }
        else if (currentState is FSMShooting) {

        }

        // ensure state Update runs
        base.Update();
    }

    public override bool is_wizard()
    {
        return true;
    }
}
