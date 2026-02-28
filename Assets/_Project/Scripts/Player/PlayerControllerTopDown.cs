using KBCore.Refs;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerControllerTopDown : ValidatedMonoBehaviour, IDamagablle
{
    [SerializeField, Self] Rigidbody2D _rb;
    [SerializeField, Self] BoxCollider2D _collider;
    [SerializeField, Self] SpriteRenderer _spriteRenderer;
    [SerializeField, Anywhere] InputReader _input;
    [SerializeField] Stats _stats;
    [SerializeField] GameObject _weaponAxis;
    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _gunSpawnPoint;
    [SerializeField] List<GameObject> _guns;


    StateMachine _stateMachine;
    StateContextNew _stateContext;
    Camera _camera;
    bool _isFiring = false;

    public InputReader Input => _input;
    public Rigidbody2D RB => _rb;
    public GameObject WeaponAxis => _weaponAxis;
    public Camera CameraMain => _camera;
    public Transform Transform => transform;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    private void OnEnable()
    {
        Input.EnablePlayerActions();
        Input.Shoot += OnShoot;
        Input.NextWeapon += OnNextWeapon;
    }
    private void Awake()
    {
        Cursor.visible = false;

        _rb.freezeRotation = true;

        _stateContext = new StateContextNew();


        UniversalState _universalState = new UniversalState(this, _stats, _stateContext);
        _stateMachine = new StateMachine(_universalState);
    }

    void Start()
    {
        _camera = Camera.main;

        _stateContext.CurrentHealth = _stats.MaxHealth;

        _stateContext.CurrentGunObject = Instantiate(_guns[0], _gunSpawnPoint.position, Quaternion.identity, _gunSpawnPoint);
        _stateContext.CurrentGunScript = _stateContext.CurrentGunObject.GetComponent<Gun>();
        _stateContext.WeaponID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isFiring)
            HandleGun();

        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
    
    private void HandleGun()
    {
        if (_stateContext.CurrentGunScript.NextShotTimer.isRunning)
            return;

        Vector2 shootDirection = (Vector2)(CameraMain.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);

        GameObject tempBullet = Instantiate(_bullet, _stateContext.CurrentGunScript.BulletSpawnPoint.position, WeaponAxis.transform.rotation);

        tempBullet.GetComponent<Bullet>().Direction = shootDirection.normalized;
        tempBullet.GetComponent<Bullet>().Shoot();
    }

    private void OnShoot(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            if (_stateContext.CurrentGunScript.Stats.IsAutomatic)
            {
                _isFiring = true;
            }
            else
            {
                _isFiring = false;
                HandleGun();
            }

        }


        if (phase == InputActionPhase.Canceled)
            _isFiring = false;  
    }   


    public void OnNextWeapon(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            ChangeWeapon((_stateContext.WeaponID + 1) % 2);
        }
    }

    private void ChangeWeapon (int id)
    {
        Quaternion rot = _stateContext.CurrentGunObject.transform.rotation;

        Destroy(_stateContext.CurrentGunObject);

        _stateContext.CurrentGunObject = Instantiate(_guns[id], _gunSpawnPoint.position, rot, _gunSpawnPoint);
        _stateContext.CurrentGunScript = _stateContext.CurrentGunObject.GetComponent<Gun>();
        _stateContext.WeaponID = id;
    }

    public void TakeDamage(int damage)
    {
        _stateContext.CurrentHealth -= damage;

        if (_stateContext.CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
    }
}
