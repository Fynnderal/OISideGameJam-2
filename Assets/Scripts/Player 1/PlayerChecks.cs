using UnityEngine;

/// <summary>
/// Performs necessary checks related to the player's state and environment.
/// Responsible for ground detection, wall detection, flip/rotation handling and utility transforms scaling.
/// </summary>
public class PlayerChecks
{
    PlayerController _player;

    StateContext StateContext => _player.StateContext;
    InputReader Input => _player.Input;
    Rigidbody2D RB => _player.RB;

    Collider2D Collider2D => _player.Collider;
    PlayerStatsBlack PlayerStatsBlack => _player.PlayerStatsBlack; 

    bool IsFacingRight { get => _player.IsFacingRight; set => _player.IsFacingRight = value; }


    bool IsGrounded => _player.IsGrounded;
    PlayerAnimationController AnimationController => _player.AnimationController;

    public PlayerChecks(PlayerController player)
    {
        _player = player;
    }

    /// <summary>
    /// Determines whether the player can enter a wall-slide state.
    /// Conditions:
    ///  - Not grounded,
    ///  - There is a wall in the direction the player is moving (touch direction * movementDirection.x > 0),
    ///  - Vertical velocity is zero or downwards (not ascending).
    /// </summary>
    public bool CanSlide()
    {
        return (!StateContext.IsGrounded && StateContext.IsTouchingWall * Input.movementDirection.x > 0 && RB.linearVelocityY <= 0f);
    }

    /// <summary>
    /// Performs a BoxCast below the player's collider to determine whether the player is grounded.
    /// Updates StateContext.IsGrounded and triggers landing effects/animation when landing occurs.
    /// </summary>
    public void GroundCheck()
    {
        // Use a slightly smaller box than the collider to avoid false positives from overlapping geometry.
        Vector2 origin = new Vector2(Collider2D.bounds.center.x, Collider2D.bounds.center.y);
        Vector2 size = new Vector2(Collider2D.bounds.size.x - 0.07f, Collider2D.bounds.size.y - 0.05f);

        // Cast down with a configured distance and check both ground and wall layers.
        var hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, PlayerStatsBlack.GroundCheckDistance, PlayerStatsBlack.GroundLayer | PlayerStatsBlack.WallLayer);

        // Update grounded flag on the shared context
        StateContext.IsGrounded = hit.collider != null;

        // If we have just landed (was not grounded before but now is), play landing feedback.
        if (!IsGrounded && StateContext.IsGrounded)
        {
            // Set appropriate fall animation depending on active suit
            if (StateContext.IsBlack)
                AnimationController.ChangeAnimation(StateContext.FallAnimationHash);
            else
                AnimationController.ChangeAnimation(StateContext.FallRedAnimationHash);

            // Play landing sound and landing particles.
            _player.PlayerSounds.PlayLandSound();
            foreach (var particle in _player.LandingParticles)
            {
                // Flip particle scale to match player facing direction so effects orient correctly.
                if (!IsFacingRight)
                    SetScale(particle.transform, -1);
                else
                    SetScale(particle.transform, 1);
                particle.Play();
            }
        }
    }

    /// <summary>
    /// Checks if the player is touching a wall on either side and updates StateContext accordingly.
    /// Results:
    ///  - IsTouchingWall =  1  if touching a wall to the right
    ///  - IsTouchingWall = -1  if touching a wall to the left
    ///  - IsTouchingWall =  0  if not touching any wall
    /// Also records LastWallHit to preserve which side was last contacted.
    /// </summary>
    public void isTouchingWall()
    {
        Vector2 center = Collider2D.bounds.center;
        Vector2 size = Collider2D.bounds.size;

        // BoxCast to the right and left using configured wall detection distance and wall layer.
        var hitRight = Physics2D.BoxCast(center, size, 0f, Vector2.right, PlayerStatsBlack.WallDetectionDistance, PlayerStatsBlack.WallLayer);
        var hitLeft = Physics2D.BoxCast(center, size, 0f, Vector2.left, PlayerStatsBlack.WallDetectionDistance, PlayerStatsBlack.WallLayer);

        if (hitRight.collider != null)
        {
            StateContext.IsTouchingWall = 1;
            StateContext.LastWallHit = 1;
        }
        else if (hitLeft.collider != null)
        {
            StateContext.IsTouchingWall = -1;
            StateContext.LastWallHit = -1;
        }
        else
            StateContext.IsTouchingWall = 0;
    }

    /// <summary>
    /// Handles player facing direction based on input. Will not flip while the player is pulling (grappling).
    /// </summary>
    public void HandleRotation()
    {
        if (StateContext.IsPulling)
            return;

        // Flip to face right when moving right and currently facing left.
        if (!IsFacingRight && Input.movementDirection.x > 0)
        {
            Flip(true);
        }
        // Flip to face left when moving left and currently facing right.
        else if (IsFacingRight && Input.movementDirection.x < 0)
        {
            Flip(false);
        }
    }

    /// <summary>
    /// Helper to flip the X scale of an arbitrary transform (multiplies x by -1).
    /// Used internally to mirror objects when player flips.
    /// </summary>
    private void FlipObject(Transform _transform)
    {
        Vector3 scale = _transform.localScale;
        scale.x *= -1;
        _transform.localScale = scale;
    }

    public void SetScale(Transform _transform, float scaleX)
    {
        Vector3 scale = _transform.localScale;
        scale.x = scaleX;
        _transform.localScale = scale;
    }

    public void Flip(bool isFacingRight)
    {
        IsFacingRight = isFacingRight;
        FlipObject(_player.transform);

    }
}
