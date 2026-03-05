using UnityEngine;
using Utilities;

public class FireTrap : MonoBehaviour
{
    [SerializeField] public static float damage = 1;
    public static float damageUpgrade = 1f;
    [SerializeField] private GameObject _fire;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IDamagablle>()?.TakeDamage((damage * damageUpgrade));

            if ((collision.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerControllerTopDown>().IsOnFire) )
            {
                collision.GetComponent<PlayerControllerTopDown>().IsOnFire = true;
                BoxCollider2D boxCollider = collision.GetComponent<BoxCollider2D>();
                GameObject temp = Instantiate(_fire, new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y), Quaternion.identity, collision.transform);
                temp.GetComponent<Fire>().parent = collision.gameObject;

            }else if ((collision.CompareTag("Enemy") && !collision.gameObject.GetComponent<EnemyBase>().IsOnFire))
            {
                collision.GetComponent<EnemyBase>().IsOnFire = true;
                BoxCollider2D boxCollider = collision.GetComponent<BoxCollider2D>();
                GameObject temp = Instantiate(_fire, new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y), Quaternion.identity, collision.transform);
                temp.GetComponent<Fire>().parent = collision.gameObject;
            }

        }
    }
}
