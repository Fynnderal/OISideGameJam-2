using UnityEngine;

/// <summary>
/// Implements the down dash state for the player.
/// </summary>
public class DownDashState : PlayerBaseState
{
    protected PlayerRedStats _playerRedStats;

    public DownDashState(PlayerController player, PlayerRedStats stats, StateContext stateContext) : base(player, stats, stateContext)
    {
        _playerRedStats = stats;
    }

    public override void OnEnter()
    {
        _playerController.RB.gravityScale = 0f;
        _playerController.RB.linearVelocity = Vector2.zero;
        _stateContext.DownDashesUsed++;
        _stateContext.IsFalling = false;


        if (_stateContext.IsBlack)
            _playerController.AnimationController.ChangeAnimation(_stateContext.DownDashBlackAnimationHash);
        else
            _playerController.AnimationController.ChangeAnimation(_stateContext.DownDashRedAnimationHash);
        
        // Reset after-image indexing (used to tint successive after-images).
        _stateContext.currentAfterImageIndex = 0;

        // Apply the dash impulse and trigger associated feedback (camera shake, sound).
        PerformDash();
    }

    /// <summary>
    /// Per-frame update: spawn after-images on a timer to create a trailing visual effect.
    /// </summary>
    public override void Update()
    {
        if (!_stateContext.AfterImageTimer.isRunning)
        {
            Color temp;
            if (_stateContext.IsBlack)
            {
                // Create an after-image using the black-suit down-dash sprite and tint it progressively.
                temp = _playerController.PlayerStatsBlack.AfterImageColor;
                temp.g -= _stateContext.currentAfterImageIndex * _playerController.PlayerStatsBlack.AfterImageColorStep;
                _playerController.AfterPerformImage(_playerController.PlayerStatsBlack.AfterImageDownDashSprite, temp, _playerController.PlayerStatsBlack.AfterImageTargetColor);
            }
            else
            {
                // Create an after-image using the red-suit down-dash settings.
                temp = _playerController.PlayerRedStats.AfterImageColor;
                temp.g -= _stateContext.currentAfterImageIndex * _playerController.PlayerRedStats.AfterImageColorStep;
                _playerController.AfterPerformImage(_playerController.PlayerStatsBlack.AfterImageDownDashSprite, temp, _playerController.PlayerRedStats.AfterImageTargetColor);
            }

            _stateContext.AfterImageTimer.Start();

        }
    }

    public override void FixedUpdate()
    {
    }

    /// <summary>
    /// Called when exiting the down-dash state.
    /// Restores gravity and clamps vertical velocity so the player doesn't exceed max fall speed.
    /// </summary>
    public override void OnExit()
    {
        // Restore the cached gravity scale so other states use normal gravity.
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;

        // Clamp the downward velocity to the player's maximum fall speed for red-suit (or base stat).
        _playerController.RB.linearVelocityY = Mathf.Max(-_playerStats.MaxFallSpeed, _playerController.RB.linearVelocityY);
    }

    /// <summary>
    /// Applies the downward dash impulse, triggers camera shake and plays dash sound.
    /// </summary>
    private void PerformDash()
    {
        // Apply a strong impulse directly downward.
        _playerController.RB.AddForce(Vector2.down * _playerStats.DashForce, ForceMode2D.Impulse);

        // Trigger a camera shake configured specifically for the down-dash (red-suit settings).
        CameraController.Instance.ScreenShake(_playerController.ImpulseSource, _playerRedStats.DownDashCameraShake);

        // Play dash audio feedback.
        _playerController.PlayerSounds.PlayDashSound();
    }
}
