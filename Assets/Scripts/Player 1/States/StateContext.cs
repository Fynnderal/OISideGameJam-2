using UnityEngine;
using Utilities;

public class StateContext
{
    public bool IsGrounded { get; set; }
    public bool IsBlack { get; set; }
    
    public bool IsDamaged { get; set; } 

    public bool IsFalling { get; set; }
    //public float CurrentMoveSpeed { get; set; }
    public float GravityScaleCached { get; set; }


    public bool InPauseMenu { get; set; }   

    public CountdownTimer JumpBufferTimer { get; set; }
    public CountdownTimer JumpCoyoteTimer { get; set; }
    public CountdownTimer ApexHangTimer { get; set; }
    public CountdownTimer SideDashTimer { get; set; }
    public CountdownTimer DownDashTimer { get; set; }
    public CountdownTimer WallTouchedBuffer { get; set; }
    public CountdownTimer HorizontalMoveBlockTimer { get; set; }
    public CountdownTimer GlidingBeforeResetTimer { get; set; }
    public CountdownTimer AfterImageTimer { get; set; }

    public int currentAfterImageIndex { get; set; }

    public bool JumpCut { get; set; }
    public bool JumpWasHeld { get; set; }
    public bool IsCoyoteTimeEnabled { get; set; }
    public bool IsJumping { get; set; }
    public bool IgnoreHorizontalMovement { get; set; }

    public bool IsWallTouchedBufferEnabled { get; set; }    

    public int JumpCount { get; set; }
    public int PreviousWallJump { get; set; }

    public float CashedLinearVelocityY { get; set; }

    public int currentCombatId { get; set; }

    public sbyte LastWallHit { get; set; }

    public int IdleAnimationHash { get; set; }
    public int RunAnimationHash { get; set; }
    public int JumpAnimationHash { get; set; }
    public int FallAnimationHash { get; set; }
    public int SideDashAnimationHash { get; set; }  
    public int GlidingAnimationHash { get; set; }
    public int GlidingRevAnimationHash { get; set; }
    public int BlackToRedAnimationHash { get; set; }
    public int IdleRedAnimationHash { get; set; }
    public int RunRedAnimationHash { get; set; }
    public int JumpRedAnimationHash { get; set; }
    public int FallRedAnimationHash { get; set; }
    public int SideDashRedAnimationHash { get; set; }
    public int RedToBlackAnimationHash { get; set; }
    public int HookSwingAnimationHash { get; set; } 
    public int PullingAnimationHash { get; set; }
    public int DownDashBlackAnimationHash { get; set; }
    public int DownDashRedAnimationHash { get; set; }   
    public int WallSlideBlackAnimationHash { get; set; }
    public int WallSlideRedAnimationHash { get; set; }  
    public int CurrentHealth { get; set; }


    public int[] CombatAnimationsHash { get; set; }
    public int[] CombatRedAnimationsHash { get; set; }
    public bool IsGliding { get; set; }
    public bool IsSliding { get; set; }
    public sbyte IsTouchingWall { get; set; }

    public CountdownTimer CombatBufferTimer { get; set; }
    public GameObject nearestAnchor { get; set; }
    public GameObject currentAnchor { get; set; }
    public bool IsSwinging { get; set; }
    
    public bool IsPulling { get; set; }
    public int PullingDirection { get; set; }
    public int SideDashesUsed { get; set; }
    public int DownDashesUsed { get; set; }
    
}

