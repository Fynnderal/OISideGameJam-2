using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] public static float damage = 1;
    public static float damageUpgrade = 1f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamagablle>()?.TakeDamage((damage * damageUpgrade));
        }
    }
}
