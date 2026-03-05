using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundDeceleration;
    [SerializeField] private float _runSpeed; 
    [SerializeField] private float _groundAccelerationPow;
    [SerializeField] private float _maxHealth;


    [Header("Animation")]
    [SerializeField] private string _runAnimName;
    [SerializeField] private string _damageAnimName;
    [SerializeField] private string _deathAnimName;
    [SerializeField] private string _runBackWardAnimName;
    [SerializeField] private string _idleAnimName;



    public float GroundAcceleration => _groundAcceleration;
    public float GroundDeceleration => _groundDeceleration;
    public float RunSpeed => _runSpeed;
    public float GroundAccelerationPow => _groundAccelerationPow;
    public float MaxHealth => _maxHealth;
    public string RunAnimName => _runAnimName; 
    public string DamageAnimName => _damageAnimName;
    public string DeathAnimName => _deathAnimName;
    public string RunBackWardAnimName => _runBackWardAnimName; 
    public string IdleAnimName => _idleAnimName;


}
