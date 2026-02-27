using UnityEngine;

public interface IEnemy
{
    // Returns true if the enemy took damage, false if it was immune
    bool TakeDamage(int damage, Suit suit, GameObject player);
}
