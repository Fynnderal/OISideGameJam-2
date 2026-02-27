using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Implements the wall sliding state for the player.
/// </summary>
public class WallSlidingState : PlayerBaseState
{
    protected PlayerStatsBlack _playerStatsBlack;
    public WallSlidingState(PlayerController playerController, PlayerStatsBlack playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext)
    {
        _playerStatsBlack = playerStats;
    }

    public override void OnEnter()
    {
        _stateContext.IsSliding = true;

        if (!_playerController.IsFacingRight)
            _playerController.PlayerChecks.SetScale(_playerController.SlidingParticles.transform, -1);
        else
            _playerController.PlayerChecks.SetScale(_playerController.SlidingParticles.transform, 1);

        _playerController.SlidingParticles.Play();  
        if (_stateContext.IsBlack)
            _playerController.AnimationController.ChangeAnimation(_stateContext.WallSlideBlackAnimationHash);
        else
            _playerController.AnimationController.ChangeAnimation(_stateContext.WallSlideRedAnimationHash);

        _playerController.RB.linearVelocityY = 0f;
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached * _playerStatsBlack.WallSlideGravityMultiplier;
    }


    public override void Update()
    {
        if (_stateContext.JumpBufferTimer.isRunning)
        {
            _stateContext.IsSliding = false;
            _stateContext.HorizontalMoveBlockTimer.Start();
        }
    }

    public override void FixedUpdate()
    {
        //WallSlide();
        _playerController.RB.linearVelocityY = Mathf.Max(_playerController.RB.linearVelocityY, -_playerStatsBlack.WallSlideMaxSpeed);
    }
    //private void WallSlide()
    //{
    //    float speedDifference = Mathf.Max(Mathf.Abs(-_playerStatsBlack.WallSlideMaxSpeed - _playerController.RB.linearVelocityY), 0.1f);

    //    float finalAcceleration = _playerController.RB.linearVelocityY > -_playerStatsBlack.WallSlideMaxSpeed ? _playerStatsBlack.WallSlideAccelerationSpeed : _playerStatsBlack.WallSlideDecelerationSpeed;


    //    _playerController.RB.linearVelocityY = Mathf.MoveTowards(_playerController.RB.linearVelocityY, -_playerStatsBlack.WallSlideMaxSpeed, Mathf.Pow(speedDifference, _playerStatsBlack.WallSlideAccelerationPow) * finalAcceleration * Time.fixedDeltaTime);
    //}

    //private void ResetWallJumpValues()
    //{
    //    _isWallSlideFalling = false;

    //}

    //private void ResetDashValues()
    //{

    //}

    //private void ResetDashes()
    //{

    //}

    public override void OnExit()
    {
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;   
        _playerController.SlidingParticles.Stop();
    }
}
