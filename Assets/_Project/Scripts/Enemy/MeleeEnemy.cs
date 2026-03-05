using KBCore.Refs;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{

    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private LayerMask _attackLayerMask;

    public override void PerformAttack()
    {
        base.PerformAttack();

        RaycastHit2D ray_hit = Physics2D.Raycast(_rb.position, _moveDirection, _attackRange, _attackLayerMask);

        if (ray_hit.collider != null)
        {
            ray_hit.collider.GetComponent<IDamagablle>()?.TakeDamage(_attackDamage);
        }   

    }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawSphere(_rb.position, _attackRange);
        //}
}
