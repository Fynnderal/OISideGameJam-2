using KBCore.Refs;
using UnityEngine;
using Utilities;

public class SinusEnemy : EnemyBase
{
    //[SerializeField] private float _projectilesNumber = 3f;
    [SerializeField, Anywhere] private GameObject _projectile;
    [SerializeField] private Transform _projectileSpawnPoint;




    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void Attack()
    {
        base.Attack();
    }

    public override void PerformAttack()
    {
        Instantiate(_projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
    }
}
