using UnityEditor;
using UnityEngine;

/// <summary>
/// Implements the gliding state for the player.
/// </summary>
public class GlidingState : HorizontalMovementState
{
    protected PlayerStatsBlack _playerStatsBlack;
    public GlidingState(PlayerController playerController, PlayerStatsBlack playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext)
    {
        _playerStatsBlack = playerStats;
    }   
    public override void OnEnter()
    {
        base.OnEnter(); 
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached * _playerStatsBlack.GlidingGravityMultiplier;

       
        _playerController.RB.linearVelocityX = Mathf.Clamp(_playerController.RB.linearVelocityX, -_playerStatsBlack.GlidingHorizontalMaxSpeed, _playerStatsBlack.GlidingHorizontalMaxSpeed);

        if (!_stateContext.GlidingBeforeResetTimer.isRunning)
        {
            // Resets speed
            _playerController.RB.linearVelocityY = 0f;
        }
        else
        {
            // While the timer is active limit downward velocity to the glide max fall speed
            if (_playerController.RB.linearVelocityY < 0)
                _playerController.RB.linearVelocityY = Mathf.Max(_playerController.RB.linearVelocityY, -_playerStatsBlack.GlidingMaxFallSpeed);
            else
                _playerController.RB.linearVelocityY = 0f;
        }
        _stateContext.GlidingBeforeResetTimer.Start();

        _playerController.AnimationController.ChangeAnimation(_stateContext.GlidingAnimationHash);
    }
    public override void Update()
    {}

    public override void FixedUpdate()
    {

        HandleMovement(_playerStatsBlack.GlidingHorizontalAcceleration, _playerStatsBlack.GlidingHorizontalDeceleration, _playerStatsBlack.GlidingHorizontalMaxSpeed, _playerStatsBlack.GlidingAccelerationPow);
        _playerController.RB.linearVelocityY = Mathf.Max(_playerController.RB.linearVelocityY, -_playerStatsBlack.GlidingMaxFallSpeed);
    }

    /// <summary>
    /// Cleanup when exiting glide: play the glide release animation, restore gravity,
    /// clear gliding flag and stop the short reset timer if the player is grounded.
    /// </summary>
    public override void OnExit()
    {
        // Play the "glide release" animation to transition out of the gliding pose.
        _playerController.AnimationController.ChangeAnimation(_stateContext.GlidingRevAnimationHash);

        // Restore gravity to the cached value so other states use normal gravity.
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;   

        // Clear the gliding flag in the shared context.
        _stateContext.IsGliding = false; 

        // If we've landed by the time we exit, stop the short timer used during glide entry.
        if (_playerController.IsGrounded)
            _stateContext.GlidingBeforeResetTimer.Stop();

        base.OnExit();
    }
}
