using UnityEngine;

public interface IEnemyAttack
{
    void Attack(PlayerController player);
    float GetAttackRange();
    float GetAttackDelay();
}
