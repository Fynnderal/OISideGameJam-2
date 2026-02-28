using UnityEngine;
using Utilities;
public class Gun : MonoBehaviour
{
    [SerializeField] GunBaseStats _stats;
    [SerializeField] Transform _bulletSpawnPoint;
    private float _timeUntilNextShot = 0;

    private CountdownTimer _nextShotTimer;


    private float _currentBulletsNumber = 0;


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
