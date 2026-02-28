using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundDeceleration;
    [SerializeField] private float _runSpeed; 
    [SerializeField] private float _groundAccelerationPow;
    [SerializeField] private int _maxHealth;



    public float GroundAcceleration => _groundAcceleration;
    public float GroundDeceleration => _groundDeceleration;
    public float RunSpeed => _runSpeed;
    public float GroundAccelerationPow => _groundAccelerationPow;
    public int MaxHealth => _maxHealth;


}
