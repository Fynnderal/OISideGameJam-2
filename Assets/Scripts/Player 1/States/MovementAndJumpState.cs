using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;


/// <summary>
/// Implements the movement and jump state for the player.
/// </summary>
public class MovementAndJumpState : HorizontalMovementState
{
    protected PlayerStatsBlack _playerBlackStats;
    public MovementAndJumpState(PlayerController playerController, PlayerStatsBlack playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext)
    {
        _playerBlackStats = playerStats;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // Subscribe to timers to react to apex hang and other timed events.
        _stateContext.ApexHangTimer.OnTimerStop += OnApexHangTimerStop;
        _stateContext.ApexHangTimer.OnTimerForcedStop += OnApexHangForcedStop;

        _stateContext.JumpCoyoteTimer.OnTimerStart += OnJumpCoyoteTimerStart;
        _stateContext.WallTouchedBuffer.OnTimerStart += OnWallTouchedBufferStart;

        //_stateContext.HorizontalMoveBlockTimer.OnTimerStop += () => _playerController.ChangeAnimation(_stateContext.IdleAnimationHash);

        // Ensure gravity is set to the cached default on entering this movement/JUMP-capable state.
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;  
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        // Allow horizontal movement by default for this frame unless some logic disables it.
        _stateContext.IgnoreHorizontalMovement = false;

        // Handle all jump logic (buffering, coyote, wall jumps, jump cut).
        UpdateJump();

        // Only perform horizontal movement if it's allowed and not blocked by a short timer.
        if (!_stateContext.IgnoreHorizontalMovement && !_stateContext.HorizontalMoveBlockTimer.isRunning)
            Move();

        // Update gravity handling (apex/hang/fall multipliers).
        UpdateGravity();
    }

    public override void OnExit()
    {
        base.OnExit();

        // Reset flags/timers and unsubscribe from events when leaving the state.
        _stateContext.IgnoreHorizontalMovement = false;
        _stateContext.IsJumping = false;
        _stateContext.JumpCoyoteTimer.Stop();
        _stateContext.ApexHangTimer.Stop();
        _stateContext.JumpBufferTimer.Stop();
        _stateContext.WallTouchedBuffer.Stop(); 
        _stateContext.HorizontalMoveBlockTimer.Stop();

        _stateContext.ApexHangTimer.OnTimerStop -= OnApexHangTimerStop;
        _stateContext.ApexHangTimer.OnTimerForcedStop -= OnApexHangForcedStop;

        _stateContext.JumpCoyoteTimer.OnTimerStart -= OnJumpCoyoteTimerStart;
        _stateContext.WallTouchedBuffer.OnTimerStart -= OnWallTouchedBufferStart;
    }


    // Timer callbacks used to react to timer events and adjust gravity/flags.
    private void OnApexHangTimerStop() => _playerController.RB.gravityScale = _stateContext.GravityScaleCached * _playerStats.FallGravityMultiplier;
    private void OnApexHangForcedStop() => _playerController.RB.gravityScale = _stateContext.GravityScaleCached;
    private void OnJumpCoyoteTimerStart() => _stateContext.IsCoyoteTimeEnabled = false;
    private void OnWallTouchedBufferStart() => _stateContext.IsWallTouchedBufferEnabled = false;

    protected override void CheckAnimations()
    {
        // Choose appropriate run/idle animation depending on grounded state, input and suit.
        if (_stateContext.IsGrounded && _playerController.Input.movementDirection.x != 0 && !_stateContext.IsJumping)
        {
            if (_stateContext.IsBlack)
                _playerController.AnimationController.ChangeAnimation(_stateContext.RunAnimationHash);
            else
                _playerController.AnimationController.ChangeAnimation(_stateContext.RunRedAnimationHash);
        }
        else if (_stateContext.IsGrounded && !_stateContext.IsJumping)
        {
            if (_stateContext.IsBlack)
                _playerController.AnimationController.ChangeAnimation(_stateContext.IdleAnimationHash);
            else
                _playerController.AnimationController.ChangeAnimation(_stateContext.IdleRedAnimationHash);

        }
        //} else if (_playerController.RB.linearVelocityY <= 0)
        //{
        //    _playerController.ChangeAnimation(_stateContext.FallAnimationHash);
        //}
    }
    private void PerformJump(byte jumpsUsed, bool idle = true)
    {
        //_playerController.ChangeAnimation(_stateContext.JumpAnimationHash, true);
        if (_stateContext.IsBlack && idle)
            _playerController.AnimationController.ChangeAnimation(_stateContext.IdleAnimationHash);
        else if (idle && !_stateContext.IsBlack)
            _playerController.AnimationController.ChangeAnimation(_stateContext.IdleRedAnimationHash);

        // Stop jump buffering and any apex hang timer when performing a jump.
        _stateContext.JumpBufferTimer.Stop();
        _stateContext.ApexHangTimer.Stop();

        _stateContext.IsJumping = true;
        _stateContext.IsCoyoteTimeEnabled = false;

        // Reset vertical velocity before applying the jump impulse for consistent jumps.
        _playerController.RB.linearVelocityY = 0;

        // Apply the upward impulse using configured jump force.
        _playerController.RB.AddForce(Vector2.up * _playerStats.JumpForce, ForceMode2D.Impulse);
        _stateContext.JumpCount += jumpsUsed;

        // Play jump audio feedback.
        _playerController.PlayerSounds.PlayJumpSound();

    }

    // Centralized jump handling called from FixedUpdate.
    private void  UpdateJump()
    {
        // If we are not grounded but coyote time is enabled and not already jumping, start the coyote timer.
        if (!_stateContext.IsGrounded && _stateContext.IsCoyoteTimeEnabled && !_stateContext.IsJumping)
            _stateContext.JumpCoyoteTimer.Start();

        // If we are not touching a wall and the wall-touch buffer is disabled, start the buffer to allow wall jump forgiveness.
        if (_stateContext.IsTouchingWall == 0 && !_stateContext.IsWallTouchedBufferEnabled && !_stateContext.IsJumping)
            _stateContext.WallTouchedBuffer.Start();


        if (_stateContext.IsBlack && _stateContext.JumpBufferTimer.isRunning && !_stateContext.IsGrounded && (_stateContext.IsTouchingWall != 0 || _stateContext.WallTouchedBuffer.isRunning))
        {
            _stateContext.JumpBufferTimer.Stop();

            // Consume remaining jumps (force to max to prevent additional mid-air jumps afterwards).
            _stateContext.JumpCount = _playerStats.JumpsAllowed;

            // Cancel horizontal velocity to ensure wall-jump impulse is consistent.
            _playerController.RB.linearVelocityX = 0;
            _playerController.RB.AddForce(Vector2.left * _stateContext.LastWallHit * _playerBlackStats.WallSlideJumpHorizontalForce, ForceMode2D.Impulse);

            // If we jumped off the right wall but are facing right, flip to face away from wall, and vice versa.
            if (_stateContext.LastWallHit > 0 && _playerController.IsFacingRight)
                _playerController.PlayerChecks.Flip(false);
            else if (_stateContext.LastWallHit < 0 && !_playerController.IsFacingRight)
                _playerController.PlayerChecks.Flip(true);

            // Briefly block horizontal input so the player could infinitely rise by using wall jump on one wall.
            _stateContext.HorizontalMoveBlockTimer.Start();

            // Switch to wall-slide animation for visual continuity, then perform the upward jump (no idle visuals).
            if (_stateContext.IsBlack)
                _playerController.AnimationController.ChangeAnimation(_stateContext.WallSlideBlackAnimationHash);
            else
                _playerController.AnimationController.ChangeAnimation(_stateContext.WallSlideRedAnimationHash);
            PerformJump(0, false);
           
        }

        // Standard buffered jump handling (ground or double-jump).
        else if (_stateContext.JumpBufferTimer.isRunning && _stateContext.JumpCount < _playerStats.JumpsAllowed)
        {
            // If we are currently jumping, grounded, or inside the coyote timer window, perform a normal jump (consumes 1).
            if (_stateContext.IsJumping || _stateContext.IsGrounded || _stateContext.JumpCoyoteTimer.isRunning)
            {
                _stateContext.JumpCoyoteTimer.Stop();
                //if (!_stateContext.IsJumping)
                //{

                    if (!_playerController.IsFacingRight)
                        _playerController.PlayerChecks.SetScale(_playerController.JumpParticles.transform, -1);
                    else
                        _playerController.PlayerChecks.SetScale(_playerController.JumpParticles.transform, 1);

                    _playerController.JumpParticles.Play();
                //}
                
                PerformJump(1);
            }
            // Jump after falling from an edge (after falling we can't make double jump (maybe change it, need more testing))
            else
            {
                PerformJump(2);
            }
        }

        // Jump cut. If the player releases the jump button during the ascent, we reduce the upward velocity.
        if (_stateContext.IsJumping && _stateContext.JumpCut && _playerController.RB.linearVelocityY >= 0)
        {
            _playerController.RB.linearVelocityY = Mathf.Min(_playerController.RB.linearVelocityY, _playerStats.JumpCutVelocity);
        }


        // Sorry for this big block of code, I'll make spesiacl method for it later
        // Edge correction. If the player is jumping and one of the top corners is hitting a platform, we correct the position to avoid getting stuck.
        //if (_stateContext.IsJumping && _playerController.RB.linearVelocityY > 0f)
        //{
        //    Vector2 topLeft = _playerController.Collider.bounds.max + Vector3.left * _playerController.Collider.bounds.size.x;
        //    Vector2 topRight = _playerController.Collider.bounds.max;


        //    var hitUpLeft = Physics2D.Raycast(topLeft, Vector2.up, _playerStats.EdgeDetectionDistance, _playerStats.GroundLayer);
        //    var hitUpRight = Physics2D.Raycast(topRight, Vector2.up, _playerStats.EdgeDetectionDistance, _playerStats.GroundLayer);

        //    // edge correction
        //    // if player's right side is hitting something and left not
        //    if (!hitUpLeft && hitUpRight && _playerController.Input.movementDirection.x <= 0f)
        //    {

        //        // Logic repeated. Need to optimize it later

        //        // We check whether there is a need for edge correction and a little bit change position to avoid getting stuck    
        //        if (!Physics2D.Raycast(topRight + Vector2.left * _playerStats.MaxEdgeCorrectionDistance, Vector2.up, _playerStats.EdgeDetectionDistance, _playerStats.GroundLayer))
        //        {
        //            Debug.Log("Edge correction right"); 
        //            _stateContext.CashedLinearVelocityY = _playerController.RB.linearVelocityY;
        //            _playerController.RB.linearVelocityY = 0f;
        //            _playerController.RB.position -= new Vector2(_playerController.Collider.bounds.max.x - hitUpRight.collider.bounds.min.x + 0.04f, 0f);
        //            _playerController.RB.linearVelocityY = _stateContext.CashedLinearVelocityY;

        //        }
        //    }
        //    // the same but for the left side
        //    else if (hitUpLeft && !hitUpRight && _playerController.Input.movementDirection.x >= 0f)
        //    {
        //        // the same logic but for the other side
        //        if (!Physics2D.Raycast(topLeft + Vector2.right * _playerStats.MaxEdgeCorrectionDistance, Vector2.up, _playerStats.EdgeDetectionDistance, _playerStats.GroundLayer))
        //        {
        //            Debug.Log("Edge correction left");
        //            _stateContext.CashedLinearVelocityY = _playerController.RB.linearVelocityY;
        //            _playerController.RB.linearVelocityY = 0f;
        //            _playerController.RB.position += new Vector2(hitUpLeft.collider.bounds.max.x - _playerController.Collider.bounds.min.x + 0.04f, 0f);
        //            _playerController.RB.linearVelocityY = _stateContext.CashedLinearVelocityY;
        //        }
        //    }

        //}

        // It was an attempt to make edge correction not only from the bottom but from the sides too. But it works not very well. It's not hard to implement but hard to balance. It's better to solve this problem by good levevl design
        //    // side platform edge correction
        //    // else if (hitDownSideLeft && _input.movementDirection.x < -0.2)
        //    // { 
        //    //     edgeCheck = Physics2D.Raycast(bottomLeft + Vector2.up * _maxEdgeCorrectionDistance, Vector2.left, _edgeDetectionDistance, _groundLayer);
        //    //     bool hitUpSideLeft = Physics2D.Raycast(topLeft, Vector2.left, _edgeDetectionDistance, _groundLayer);

        //    //     if (!hitUpSideLeft && !edgeCheck)
        //    //     {
        //    //         Debug.Log("Edge correction left");
        //    //         _rb.MovePosition((Vector2)transform.position + Vector2.up * _maxEdgeCorrectionDistance);
        //    //     }
        //    // }
        //    //else if (hitDownSideRight && _input.movementDirection.x > 0.2)
        //    // {
        //    //     edgeCheck = Physics2D.Raycast(bottomRight + Vector2.up * _maxEdgeCorrectionDistance, Vector2.right, _edgeDetectionDistance, _groundLayer);
        //    //     bool hitUpSideRight = Physics2D.Raycast(topRight, Vector2.right, _edgeDetectionDistance, _groundLayer);
        //    //     if (!hitUpSideRight && !edgeCheck)
        //    //     {
        //    //         Debug.Log("Edge correction right");
        //    //         _rb.MovePosition((Vector2)transform.position + Vector2.up * _maxEdgeCorrectionDistance);
        //    //     }
        //    // }

        //}


        //After landing reset all variables
        if (_stateContext.IsGrounded && _playerController.RB.linearVelocityY <= 0)
        {
                _stateContext.JumpCount = 0;
            _stateContext.ApexHangTimer.Stop();
            _stateContext.JumpCoyoteTimer.Stop();
            _stateContext.WallTouchedBuffer.Stop();
            _stateContext.HorizontalMoveBlockTimer.Stop();
            _stateContext.IsCoyoteTimeEnabled = true;
            _stateContext.IsWallTouchedBufferEnabled = true;
            _stateContext.IsJumping = false;
        }

    }


    // Override gravity logic to handle ascending, apex hang and falling with different gravity multipliers.
    protected override void UpdateGravity()
    {
        // When moving up, use cached gravity and clear falling flag.
        if (_playerController.RB.linearVelocityY > 0)
        {
            _playerController.RB.gravityScale = _stateContext.GravityScaleCached;
            _stateContext.IsFalling = false;
        }
        else if (!_stateContext.IsFalling)
        {
            //if (_isTouchingWall && !_isGrounded)
            //{
            //    _rb.gravityScale *= _wallSlideGravityMultiplier;
            //}
            _stateContext.IsFalling = true;
            _playerController.RB.gravityScale = _stateContext.GravityScaleCached * _playerStats.FallGravityMultiplier;
            _stateContext.ApexHangTimer.Start();

        }

        // If apex hang timer is running, zero gravity to create a brief hang at the jump apex.
        if (_stateContext.ApexHangTimer.isRunning)
        {
            _playerController.RB.gravityScale = 0;
        }

    }

}
