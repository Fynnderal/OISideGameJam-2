using KBCore.Refs;
using UnityEngine;
using Utilities;
public class Gun : ValidatedMonoBehaviour
{
    [SerializeField] GunBaseStats _stats;
    [SerializeField] Transform _bulletSpawnPoint;
    [SerializeField, Self] Animator _animator;

    private CountdownTimer _nextShotTimer;


    private float _currentBulletsNumber = 0;
    public Animator Animator => _animator;


    public float CurrentBulletsNumber => _currentBulletsNumber;
    public CountdownTimer NextShotTimer => _nextShotTimer;
    public GunBaseStats Stats => _stats;
    public Transform BulletSpawnPoint => _bulletSpawnPoint;
    void Start()
    {
        _nextShotTimer = new CountdownTimer(_stats.TimeBetweenShots);
    }

    void Update()
    {
        _nextShotTimer.Tick(Time.deltaTime);
    }


}
