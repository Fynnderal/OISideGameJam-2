using UnityEngine;
using Utilities;

public class Fire : MonoBehaviour
{
    [SerializeField] private GameObject _fire;
    [SerializeField] public static float _lifeTime = 3f; //Can increase fire lifetime as upgrade. // Can increasse damage for spikes and fire as upgrade.
    [SerializeField] public static float _damage = 1;
    public static float damageUpgrade = 1f;
    public static float durationUpgrade = 1f;

    [SerializeField] private float _timeBetweenDamage = 0.5f;




    private CountdownTimer _timer;
    private CountdownTimer _damageTimer;
    public GameObject parent { get; set; }
    private void Start()
    {
        _timer = new CountdownTimer(_lifeTime * durationUpgrade);
        _damageTimer = new CountdownTimer(_timeBetweenDamage);

        _timer.OnTimerStop += () => {
            PlayerControllerTopDown player;
            EnemyBase enemy;
            if (parent.TryGetComponent<PlayerControllerTopDown>(out player))
            {
                player.IsOnFire = false;
            }
            else if (parent.TryGetComponent<EnemyBase>(out enemy))
            {
                enemy.IsOnFire = false;
            }
            Destroy(gameObject);
            };
        _timer.Start();
        _damageTimer.Start(); 
    }

    private void Update()
    {
        _timer.Tick(Time.deltaTime);
        _damageTimer.Tick(Time.deltaTime);
        if (!_damageTimer.isRunning)
        {
            parent.GetComponent<IDamagablle>()?.TakeDamage((_damage * damageUpgrade));
            _damageTimer.Start();   
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {

            if (collision.gameObject == parent) return;
            
            if ((collision.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerControllerTopDown>().IsOnFire))
            {
                collision.GetComponent<PlayerControllerTopDown>().IsOnFire = true;
                BoxCollider2D boxCollider = collision.GetComponent<BoxCollider2D>();
                GameObject temp = Instantiate(_fire, new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y), Quaternion.identity, collision.transform);
                temp.GetComponent<Fire>().parent = collision.gameObject;

            }
            else if ((collision.CompareTag("Enemy") && !collision.gameObject.GetComponent<EnemyBase>().IsOnFire))
            {
                collision.GetComponent<EnemyBase>().IsOnFire = true;
                BoxCollider2D boxCollider = collision.GetComponent<BoxCollider2D>();
                GameObject temp = Instantiate(_fire, new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y), Quaternion.identity, collision.transform);
                temp.GetComponent<Fire>().parent = collision.gameObject;
            }

        }
    }
}
