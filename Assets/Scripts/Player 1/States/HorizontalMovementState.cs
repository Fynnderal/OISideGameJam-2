using UnityEngine;

/// <summary>
/// Implements the horizontal movement state for the player.
/// </summary>
public class HorizontalMovementState : PlayerBaseState
{
    protected HorizontalMovementState(PlayerController playerController, PlayerBaseStats playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext) { }

    public override void OnEnter() {
        _stateContext.IsFalling = false;
    }

    public override void Update() {
        CheckAnimations();
    }

    public override void FixedUpdate() {
        Move(); 
        UpdateGravity();
    }
    public override void OnExit() {
        _playerController.PlayerSounds.StopWalkingSound();
    }

    protected virtual void CheckAnimations()
    {

    }
    protected void Move()
    {

        // Depending on whether the player is grounded or in the air, we handle movement differently    
        if (_stateContext.IsGrounded)
        {
            HandleMovement(_playerStats.GroundAcceleration, _playerStats.GroundDeceleration, _playerStats.RunSpeed, _playerStats.GroundAccelerationPow);
            if (_playerController.Input.movementDirection.x != 0)
                _playerController.PlayerSounds.UpdateWalkingSound(true);
        }
        else
        {
            HandleMovement(_playerStats.AirAcceleration, _playerStats.AirDeceleration, _playerStats.AirSpeed, _playerStats.AirAccelerationPow);
            _playerController.PlayerSounds.StopWalkingSound();
        }
        // Cap the fall speed to prevent excessive falling speed    
        if (_stateContext.IsFalling)
            _playerController.RB.linearVelocityY = Mathf.Max(_playerController.RB.linearVelocityY, -_playerStats.MaxFallSpeed);
    }

    protected void HandleMovement(float acceleration, float deceleration, float speed, float accelerationPow)
    {
        float targetSpeed = _playerController.Input.movementDirection.x * speed;
        float speedDifference = Mathf.Max(Mathf.Abs(targetSpeed - _playerController.RB.linearVelocityX), 0.01f);

        //float speedDifference = Mathf.Max(Mathf.Abs(targetSpeed - _playerController.CurrentMoveSpeed), 0.01f);

        float finalAcceleration = _playerController.Input.movementDirection.x != 0 ? acceleration : deceleration;
        _playerController.RB.linearVelocityX = Mathf.MoveTowards(_playerController.RB.linearVelocityX, targetSpeed, Mathf.Pow(speedDifference, accelerationPow) * finalAcceleration * Time.fixedDeltaTime);

        //_playerController.CurrentMoveSpeed = Mathf.MoveTowards(_playerController.CurrentMoveSpeed, targetSpeed, Mathf.Pow(speedDifference, _playerStats.AccelerationPow) * finalAcceleration * Time.fixedDeltaTime);

        //_playerController.RB.linearVelocityX = _playerController.CurrentMoveSpeed;
    }


    protected virtual void UpdateGravity()
    {
        // We have different gravity scale when the player is falling down  

        if (!_stateContext.IsFalling && _playerController.RB.linearVelocityY < 0)
        {
            _stateContext.IsFalling = true;
            _playerController.RB.gravityScale *= _playerStats.FallGravityMultiplier;
        }
       // If vertical velocity is non-negative (moving up or stationary), restore cached gravity and clear falling flag.
        else if (_playerController.RB.linearVelocityY >= 0)
        {
            _playerController.RB.gravityScale = _stateContext.GravityScaleCached;
            _stateContext.IsFalling = false;
        }
    }
}
