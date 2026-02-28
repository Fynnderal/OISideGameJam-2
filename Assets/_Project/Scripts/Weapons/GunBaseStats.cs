using UnityEngine;

[CreateAssetMenu(fileName = "GunBaseStats", menuName = "Scriptable Objects/GunBaseStats")]
public class GunBaseStats : ScriptableObject
{
    [SerializeField] protected float _timeBetweenShots;
    [SerializeField] protected int _damage;
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected float _maxBulletsInMag;
    [SerializeField] protected float _reloadTime;
    [SerializeField] protected float _angleRandomness;
    [SerializeField] protected bool _isAutomatic;


    public float TimeBetweenShots => _timeBetweenShots;
    public int Damage => _damage;
    public GameObject Bullet => _bullet;
    public float MaxBulletsInMag => _maxBulletsInMag;
    public float ReloadTime => _reloadTime;
    public float AngleRandomness => _angleRandomness; 
    public bool IsAutomatic => _isAutomatic;
}
