
using KBCore.Refs;
using Unity.Hierarchy;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStatsBlack : PlayerBaseStats
{
    [Header("Gliding Settings")]
    [SerializeField] float _glidingGravityMultiplier;
    [SerializeField] float _glidingMaxFallSpeed;
    [SerializeField] float _glidingHorizontalMaxSpeed;
    [SerializeField] float _glidingHorizontalAcceleration;
    [SerializeField] float _glidingHorizontalDeceleration;
    [SerializeField] float _glidingAccelerationPow;

    [Header("Wall sliding settings")]
    [SerializeField] float _wallDetectionHeightMultiplier;
    [SerializeField] float _wallDetectionDistance;
    [SerializeField] float _wallSlideGravityMultiplier;
    [SerializeField] float _wallSlideMaxSpeed;
    [SerializeField] float _wallSlideJumpHorizontalForce;
    [SerializeField] float _wallTouchBufferTime;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] float _horizontalMoveBlockTime;

    [Header("Cinemachine Camera Settings")]
    [SerializeField] CameraShakeSettings _leftDashCameraShake;
    [SerializeField] CameraShakeSettings _rightDashCameraShake;
    [SerializeField] CameraShakeSettings _chargingCameraShake;
    [SerializeField] CameraShakeSettings _piercingShotCameraShake;

    [Header("Dash Settings")]
    [SerializeField] protected int _sideDashesAllowed = 1;



    public CameraShakeSettings LeftDashCameraShake => _leftDashCameraShake;
    public CameraShakeSettings RightDashCameraShake => _rightDashCameraShake;

   

    public float GlidingGravityMultiplier => _glidingGravityMultiplier;
    public float GlidingMaxFallSpeed => _glidingMaxFallSpeed;
    public float GlidingHorizontalMaxSpeed => _glidingHorizontalMaxSpeed;
    public float GlidingHorizontalAcceleration => _glidingHorizontalAcceleration;
    public float GlidingHorizontalDeceleration => _glidingHorizontalDeceleration;
    public float GlidingAccelerationPow => _glidingAccelerationPow;
    public CameraShakeSettings ChargingCameraShake => _chargingCameraShake; 
    public CameraShakeSettings PiercingShotCameraShake => _piercingShotCameraShake;

    public float WallDetectionHeightMultiplier => _wallDetectionHeightMultiplier;
    public float WallDetectionDistance => _wallDetectionDistance;
    public float WallSlideGravityMultiplier => _wallSlideGravityMultiplier;
    public float WallSlideMaxSpeed => _wallSlideMaxSpeed;
    public float WallSlideJumpHorizontalForce => _wallSlideJumpHorizontalForce;
    public float WallTouchBufferTime => _wallTouchBufferTime;
    public LayerMask WallLayer => _wallLayer;
    public float HorizontalMoveBlockTime => _horizontalMoveBlockTime;
    public int SideDashesAllowed => _sideDashesAllowed;


}
