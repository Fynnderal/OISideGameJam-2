using UnityEngine;
using UnityEngine.Rendering;

public class MeleeEnemy : EnemyBase
{

    [SerializeField] private int _attackDamage = 20;    
    override protected void Start()
    {
        base.Start();
    }
    override protected void Update()
    {
        base.Update();

        if (_distance.magnitude <= _attackRadius)
        {
            Attack();
        }
    }


}
