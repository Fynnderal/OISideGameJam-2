using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRedStats", menuName = "Scriptable Objects/PlayerRedStats")]
public class PlayerRedStats : PlayerBaseStats
{
    [Header("Hook settings")]
    [SerializeField] private float _hookMaxDistance;
    [SerializeField] private float _hookForce;
    [SerializeField] private string _hookSwingAnimationName;
    [SerializeField] private string _hookPullingAnimationName;

    [Header("Swing settings")]
    [SerializeField] private string _anchorTag;
    [SerializeField] private float _swingForce;
    [SerializeField] private float _distance;
    [SerializeField] private float _dampingRatio;
    [SerializeField] private float _Frequency;
    [SerializeField] private float _swingSpeed;


    [Header("Camera Shake Settings")]
    [SerializeField] CameraShakeSettings _downDashCameraShake;

    [Header("Dash Settings")]
    [SerializeField] private int _downDashesAllowed = 1;

    public string AnchorTag => _anchorTag;
    public string HookPullingAnimationName => _hookPullingAnimationName;
    public string HookSwingAnimationName => _hookSwingAnimationName;
    public float HookMaxDistance => _hookMaxDistance;
    public float HookForce => _hookForce;
    public float SwingForce => _swingForce;
    public float Distance => _distance;
    public float DampingRatio => _dampingRatio;
    public float Frequency => _Frequency;
    public CameraShakeSettings DownDashCameraShake => _downDashCameraShake;
    public int DownDashesAllowed => _downDashesAllowed;
    public float MaxSwingSpeed => _swingSpeed;

}
