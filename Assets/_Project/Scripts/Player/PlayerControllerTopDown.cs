using KBCore.Refs;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;



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
    [SerializeField, Self] Animator _animator;
    [SerializeField] float _damageFlashDuration = 0.1f;

    [SerializeField] int _pistolDamage = 10;
    [SerializeField] int _smgDamage = 10;
    [SerializeField] public float _pistolDamageUpgrade = 1f;
    [SerializeField] public float _smgDamageUpgrade = 1f;

    [SerializeField] GameObject _pistolTrace;
    [SerializeField] GameObject _smgTrace;

    [SerializeField] Image _healthBarFill;
    [SerializeField] CameraShakeSettings _pistolshake;
    [SerializeField] CameraShakeSettings _smgShake;
    [SerializeField] CameraShakeSettings _pistolShakeLeft;
    [SerializeField] CameraShakeSettings _smgShakeLeft;
    

    [SerializeField, Self] CinemachineImpulseSource _impulseSource;

    [SerializeField] PlayerSounds _playerSounds;

    StateMachine _stateMachine;
    StateContextNew _stateContext;
    bool _isFiring = false;

    [Header("Loader")]
    public bool newGame = true;
    bool IsDead { get; set; }
    public InputReader Input => _input;
    public Rigidbody2D RB => _rb;
    public GameObject WeaponAxis => _weaponAxis;
    public Transform Transform => transform;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public AnimationController AnimationController => _animationController;
    private AnimationController _animationController;
    private CountdownTimer _damageFlashTimer;
    [SerializeField] LevelMusic _levelMusic;
    [SerializeField] InGameController _inGameController;

    [SerializeField] GameObject _wavesManager; 
    public bool IsOnFire { get; set; }

    bool Paused = false;
    private void OnEnable()
    {
        if (!IsDead)
        {
            Input.EnablePlayerActions();
            Input.EnableUI();
        }
        Input.Shoot += OnShoot;
        Input.NextWeapon += OnNextWeapon;
        Input.PauseMenu += OnPause;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Input.Shoot -= OnShoot;
        Input.NextWeapon -= OnNextWeapon;
        Input.PauseMenu -= OnPause;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Input.DisablePlayerActions();
        Input.DisableUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
    private void Awake()
    {





        _levelMusic.IsInCombat = true;
        _animationController = new AnimationController(_animator);

        _stateContext = new StateContextNew();

        _stateContext.RunAnimHash = Animator.StringToHash(_stats.RunAnimName);
        _stateContext.RunBackwardsAnimHash = Animator.StringToHash(_stats.RunBackWardAnimName);
        _stateContext.DamageAnimHash = Animator.StringToHash(_stats.DamageAnimName);
        _stateContext.DeathAnimHash = Animator.StringToHash(_stats.DeathAnimName);
        _stateContext.IdleAnimHash = Animator.StringToHash(_stats.IdleAnimName);

        _damageFlashTimer = new CountdownTimer(_damageFlashDuration);

        Cursor.visible = false;

        _rb.freezeRotation = true;



        UniversalState _universalState = new UniversalState(this, _stats, _stateContext);
        _stateMachine = new StateMachine(_universalState);

        if (newGame)
        {
            Cursor.visible = true ;
            IsDead = true;
            Input.DisablePlayerActions();
            Input.DisableUI();
            _wavesManager.SetActive(false);
        }
    }

    public void ExitGreetings()
    {
        Input.EnablePlayerActions();
        IsDead = false;
        Input.EnableUI();
        Cursor.visible = false;
        _wavesManager.SetActive(true);
    }

    void Start()
    {

        _stateContext.CurrentHealth = _stats.MaxHealth;

        _stateContext.CurrentGunObject = _guns[0];
        _guns[0].SetActive(true);

        _bullet.GetComponent<Bullet>().Damage = (_pistolDamage * _pistolDamageUpgrade);
        _stateContext.CurrentGunScript = _stateContext.CurrentGunObject.GetComponentInChildren<Gun>();
        _stateContext.WeaponID = 0;

        _healthBarFill.fillAmount = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead || Paused)
            return;

        if (_isFiring)
            HandleGun();

        _damageFlashTimer.Tick(Time.deltaTime);
        _stateMachine.Update();

        _healthBarFill.fillAmount = Mathf.Clamp(_stateContext.CurrentHealth, 0, _stats.MaxHealth) / _stats.MaxHealth;
    }

    private void FixedUpdate()
    {
        if (IsDead || Paused)
            return;

        _stateMachine.FixedUpdate();
    }
    
    public void Pause(bool pause)
    {
        if (!pause)
        {
            Paused = false;
            Cursor.visible = false;
            Input.EnablePlayerActions();
            _inGameController.ResumeGame();

        }
        else
        {
            Paused = true;
            Input.DisablePlayerActions();
            Cursor.visible = true;
            _inGameController.PauseGame();
        }
    }
    void OnPause(InputActionPhase phase)
    {
        if (phase != InputActionPhase.Started)
            return;

        Debug.Log("Pause button pressed. Current paused state: " + Paused);
        if (Paused)
        {
            Pause(false);
        }
        else
        {
            Pause(true);
        }
    }
    private IEnumerator CreateTrace(GameObject trace)
    {
        trace.SetActive(true); 
        yield return new WaitForSeconds(0.05f);
        trace.SetActive(false);
    }
    private void HandleGun()
    {
        if (_stateContext.CurrentGunScript.NextShotTimer.isRunning)
            return;

        Vector2 shootDirection = (Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);

        GameObject tempBullet = Instantiate(_bullet, _stateContext.CurrentGunScript.BulletSpawnPoint.position, WeaponAxis.transform.rotation);

        if (_stateContext.WeaponID == 0)
        {
            _stateContext.CurrentGunScript.Animator.Play("Pistol", 0, 0f);
            tempBullet.GetComponent<Bullet>().Damage = (_pistolDamage * _pistolDamageUpgrade);
            StartCoroutine(CreateTrace(_pistolTrace));
            Debug.Log("Pistol damage " + _pistolDamage * _pistolDamageUpgrade);
            _playerSounds.PlayPistolSound();

            if (_spriteRenderer.flipX)
                CameraController.Instance.ScreenShake(_impulseSource, _pistolshake, true);
            else                 CameraController.Instance.ScreenShake(_impulseSource, _pistolShakeLeft, true);


        }
        else if (_stateContext.WeaponID == 1)
        {
            StartCoroutine(CreateTrace(_smgTrace));

            _stateContext.CurrentGunScript.Animator.Play("Smg", 0, 0f);
            tempBullet.GetComponent<Bullet>().Damage = (_smgDamage * _smgDamageUpgrade);

            if (_spriteRenderer.flipX)
            CameraController.Instance.ScreenShake(_impulseSource, _smgShake, true);
            else CameraController.Instance.ScreenShake(_impulseSource, _smgShakeLeft, true);
            _playerSounds.PlaySmgSound();
            Debug.Log("Smg damage " + _smgDamage * _smgDamageUpgrade);

        }
        _stateContext.CurrentGunScript.NextShotTimer.Start();
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

        _stateContext.CurrentGunObject.SetActive(false);

        _stateContext.CurrentGunObject = _guns[id];
        _guns[id].SetActive(true);
        _stateContext.CurrentGunScript = _stateContext.CurrentGunObject.GetComponentInChildren<Gun>();
        _stateContext.WeaponID = id;
    }

    public void TakeDamage(float damage)
    {
        if (_damageFlashTimer.isRunning)
        {

            return;
        }

        _stateContext.CurrentHealth -= damage;

        if (_stateContext.CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(TakeDamageCoroutine());
        }
    }

    private IEnumerator TakeDamageCoroutine()
    {
        _damageFlashTimer.Start();
        _animationController.ChangeAnimation(_stateContext.DamageAnimHash);
        _stateContext.isDamaged = true;
        yield return new WaitForSeconds(_damageFlashDuration);
        _stateContext.isDamaged = false;

    }


    IEnumerator DeathMenu()
    {
        yield return new WaitForSeconds(0.8f);
        _inGameController.ShowDeathMenu();
    }
    private void Die()
    {
        _healthBarFill.fillAmount = 0f;
        _animationController.ChangeAnimation(_stateContext.DeathAnimHash);
        IsDead = true;
        _rb.linearVelocity = Vector2.zero;
        _stateContext.CurrentGunObject.SetActive(false);
        Input.DisableUI();
        Input.DisablePlayerActions();

        StartCoroutine(DeathMenu());

    }
}
