using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utilities;


/// <summary>
/// Implements the dash state for the player.
/// </summary>
public class DashState : PlayerBaseState
{
    protected PlayerStatsBlack _playerStatsBlack;
    // For now I didn't implement any dash count limit, will do it later.
    public DashState(PlayerController player, PlayerStatsBlack stats, StateContext stateContext) : base(player, stats, stateContext) { 
        _playerStatsBlack = stats;
    }


    /// <summary>
    /// Called when the state becomes active.
    /// - Disables gravity and zeroes velocity so dash impulse behaves deterministically.
    /// - Increments side dash usage counters and clears falling flag.
    /// - Plays dash particles when grounded and switches to appropriate dash animation.
    /// - Begins the dash impulse and triggers camera shake and sound.
    /// </summary>
    public override void OnEnter()
    {
        // Remove gravity while dashing so vertical motion is controlled by the dash logic.
        _playerController.RB.gravityScale = 0;

        // Clear existing velocity so dash impulse produces consistent result.
        _playerController.RB.linearVelocity = Vector2.zero; 

        // Count this dash usage (used elsewhere for limits/UI).
        _stateContext.SideDashesUsed++;

        // We're not in a falling state during an initiated dash.
        _stateContext.IsFalling = false;

        // If starting the dash from the ground, orient and play dash particle effect.
        if (_stateContext.IsGrounded)
        {
            if (!_playerController.IsFacingRight)
                _playerController.PlayerChecks.SetScale(_playerController.DashParticles.transform, -1);
            else
                _playerController.PlayerChecks.SetScale(_playerController.DashParticles.transform, 1);

            _playerController.DashParticles.Play();
        }

        // Select the appropriate animation depending on current suit (black/red).
        if (_stateContext.IsBlack)
            _playerController.AnimationController.ChangeAnimation(_stateContext.SideDashAnimationHash);
        else
           _playerController.AnimationController.ChangeAnimation(_stateContext.SideDashRedAnimationHash);

        // Reset after-image index used to tint successive after-images.
        _stateContext.currentAfterImageIndex = 0;

        // Apply the dash impulse and associated audio/visual feedback.
        PerformDash();
    } 

    /// <summary>
    /// Frame update for the dash state.
    /// - Stops dash particles if horizontal movement stops or the player is airborne.
    /// - Creates after-image sprites on a timer to produce the trailing visual effect.
    /// </summary>
    public override void Update()
    {
        // If horizontal velocity becomes zero (blocked) or we leave the ground, stop the dash particles.
        if (_playerController.RB.linearVelocityX == 0 || !_stateContext.IsGrounded)
        {
            _playerController.DashParticles.Stop();
        }

        // Handle after-image spawning when the timer is ready.
        if (!_stateContext.AfterImageTimer.isRunning)
        {
            Color temp;
            // Choose color and tinting steps depending on active suit.
            if (_stateContext.IsBlack)
            {
                temp = _playerStatsBlack.AfterImageColor;
                temp.g -= _stateContext.currentAfterImageIndex * _playerStatsBlack.AfterImageColorStep;
                _playerController.AfterPerformImage(_playerController.PlayerStatsBlack.AfterImageSideDashSprite, temp, _playerStatsBlack.AfterImageTargetColor);
            }
            else
            {
                temp = _playerController.PlayerRedStats.AfterImageColor;
                temp.g -= _stateContext.currentAfterImageIndex * _playerController.PlayerRedStats.AfterImageColorStep;
                _playerController.AfterPerformImage(_playerController.PlayerStatsBlack.AfterImageSideDashSprite, temp, _playerController.PlayerRedStats.AfterImageTargetColor);
            }
            
            // Restart the after-image timer so images are spaced correctly.
            _stateContext.AfterImageTimer.Start();
        }
    }

    // No physics logic required this state's FixedUpdate; movement is handled via impulse and RB settings.
    public override void FixedUpdate()
    {
    }

    /// <summary>
    /// Called when the dash state exits.
    /// - Restores the cached gravity scale.
    /// - Clamps horizontal velocity to the normal run speed so momentum doesn't exceed intended limits.
    /// </summary>
    public override void OnExit()
    {
        // Restore gravity to previously cached value.
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;

        // Clamp horizontal velocity to the player's run speed while preserving direction.
        if (_playerController.RB.linearVelocityX >= 0)
        {
            _playerController.RB.linearVelocityX = Mathf.Min(_playerStats.RunSpeed, _playerController.RB.linearVelocityX);
        }
        else
        {
            _playerController.RB.linearVelocityX = Mathf.Max(-_playerStats.RunSpeed, _playerController.RB.linearVelocityX);
        }
    }



    /// <summary>
    /// Applies the dash impulse in the facing direction, triggers camera shake and plays dash sound.
    /// </summary>
    private void PerformDash()
    {
        if (_playerController.IsFacingRight)
        {
            // Impulse to the right
            _playerController.RB.AddForce(Vector2.right * _playerStats.DashForce, ForceMode2D.Impulse);

            // Trigger a camera shake configured for the right dash.
            CameraController.Instance.ScreenShake(_playerController.ImpulseSource, _playerStatsBlack.RightDashCameraShake);
        }
        else
        {
            // Impulse to the left
            _playerController.RB.AddForce(Vector2.left * _playerStats.DashForce, ForceMode2D.Impulse);

            // Trigger a camera shake configured for the left dash.
            CameraController.Instance.ScreenShake(_playerController.ImpulseSource, _playerStatsBlack.LeftDashCameraShake);
        }

        // Play audio feedback for the dash.
        _playerController.PlayerSounds.PlayDashSound();
    }

}
