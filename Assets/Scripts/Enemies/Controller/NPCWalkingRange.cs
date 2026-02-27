using UnityEngine;

public class NPCWalkingRange : NPCRange
{


    [SerializeField] public GameObject gun;

   

    private void Start()
    {
        detectionCollider.isTrigger = true;
            type = EnemyType.WALKINGRANGE;
        FSMIdle = new FSMPatrol(this, animator);
        gun.SetActive(false);
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
