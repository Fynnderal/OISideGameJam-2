using Utilities;
using UnityEngine;
using UnityEngine.UIElements;

public class RangeEnemy : EnemyBase
{
    [SerializeField] private float _projectilesNumber = 3f;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _attackCooldown = 3f;


    private float _angleStep;
    float _angle = 0f;
    CountdownTimer _attackTimer;

    protected override void Start()
    {
        base.Start();
        _angleStep = 360f / _projectilesNumber; 
        _attackTimer = new CountdownTimer(_attackCooldown);
    }

    protected override void Update()
    {
        base.Update();

        if (_distance.magnitude <= _attackRadius)
        {
            if (!_attackTimer.isRunning)
            {
                Attack();
                _attackTimer.Start();
            }
        }

        _attackTimer.Tick(Time.deltaTime);

    }



    protected override void Attack()
    {
        for (int i = 0; i < _projectilesNumber; i++)
        {
            float rad = Mathf.Deg2Rad * _angle;
            Vector2 shootDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject temp = Instantiate(_projectile, transform.position, Quaternion.identity);
            temp.GetComponent<Projectile>().Shoot(shootDirection);

            _angle += _angleStep;   
        }


        _angle = 0f;
    }
}
