using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeSettings", menuName = "Scriptable Objects/CameraShakeSettings")]
public class CameraShakeSettings : ScriptableObject
{
    [Header("Impulse Listener Settings")]
    [SerializeField] private float _amplitude;
    [SerializeField] private float _frequency;
    [SerializeField] private float _durationListener;


    [Header("Impulse Source Settings")]
    [SerializeField] private float durationSource;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private AnimationCurve _impulseCurve;
    [SerializeField] private float _impulseForce;


    public float Amplitude { get => _amplitude; set => _amplitude = value; }
    public float Frequency { get => _frequency; set => _frequency = value; }
    public float DurationListener { get => _durationListener; set => _durationListener = value; }
    
    public float DurationSource { get => durationSource; set => durationSource = value; } 
    public Vector3 Velocity { get => _velocity; set => _velocity = value; }
    public AnimationCurve ImpulseCurve { get => _impulseCurve; set => _impulseCurve = value; }
    public float ImpulseForce { get => _impulseForce; set => _impulseForce = value; }
}
