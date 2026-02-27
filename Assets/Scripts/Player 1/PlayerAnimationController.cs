using UnityEngine;


/// <summary>
/// Implements the animation controller for the player.
/// </summary>
public class PlayerAnimationController
{
    PlayerController _player;
    Animator _animator;

    private int _currentAnimationHash;

    StateContext StateContext => _player.StateContext;
    PlayerStatsBlack PlayerStatsBlack => _player.PlayerStatsBlack;
    PlayerRedStats PlayerRedStats => _player.PlayerRedStats;

    public int CurrentAnimationHash => _currentAnimationHash;   
    public PlayerAnimationController(PlayerController player)
    {
        _player = player;
        _animator = player.Animator;
        _currentAnimationHash = -1;
    }

    public void Init()
    {
        StateContext.IdleAnimationHash = Animator.StringToHash(PlayerStatsBlack.IdleAnimationName);
        StateContext.RunAnimationHash = Animator.StringToHash(PlayerStatsBlack.RunAnimationName);
        StateContext.JumpAnimationHash = Animator.StringToHash(PlayerStatsBlack.JumpAnimationName);
        StateContext.FallAnimationHash = Animator.StringToHash(PlayerStatsBlack.FallAnimationName);
        StateContext.SideDashAnimationHash = Animator.StringToHash(PlayerStatsBlack.SideDashAnimationName);
        StateContext.GlidingAnimationHash = Animator.StringToHash(PlayerStatsBlack.GlidingAnimationName);
        StateContext.GlidingRevAnimationHash = Animator.StringToHash(PlayerStatsBlack.GlidingRevAnimationName);
        StateContext.BlackToRedAnimationHash = Animator.StringToHash(PlayerStatsBlack.ChangeSuit);
        StateContext.IdleRedAnimationHash = Animator.StringToHash(PlayerRedStats.IdleAnimationName);
        StateContext.RunRedAnimationHash = Animator.StringToHash(PlayerRedStats.RunAnimationName);
        StateContext.JumpRedAnimationHash = Animator.StringToHash(PlayerRedStats.JumpAnimationName);
        StateContext.FallRedAnimationHash = Animator.StringToHash(PlayerRedStats.FallAnimationName);
        StateContext.SideDashRedAnimationHash = Animator.StringToHash(PlayerRedStats.SideDashAnimationName);
        StateContext.RedToBlackAnimationHash = Animator.StringToHash(PlayerRedStats.ChangeSuit);
        StateContext.HookSwingAnimationHash = Animator.StringToHash(PlayerRedStats.HookSwingAnimationName);
        StateContext.PullingAnimationHash = Animator.StringToHash(PlayerRedStats.HookPullingAnimationName);
        StateContext.DownDashBlackAnimationHash = Animator.StringToHash(PlayerStatsBlack.DownDashAnimationName);
        StateContext.DownDashRedAnimationHash = Animator.StringToHash(PlayerRedStats.DownDashAnimationName);
        StateContext.WallSlideBlackAnimationHash = Animator.StringToHash(PlayerStatsBlack.WallSlideAnimationName);
        StateContext.WallSlideRedAnimationHash = Animator.StringToHash(PlayerRedStats.WallSlideAnimationName);


        int firstCombatAttack = Animator.StringToHash(PlayerStatsBlack.CombatAttack1AnimationName);
        int secondCombatAttack = Animator.StringToHash(PlayerStatsBlack.CombatAttack2AnimationName);
        int thirdCombatAttack = Animator.StringToHash(PlayerStatsBlack.CombatAttack3AnimationName);
        StateContext.CombatAnimationsHash = new int[] { firstCombatAttack, secondCombatAttack, thirdCombatAttack };

        firstCombatAttack = Animator.StringToHash(PlayerRedStats.CombatAttack1AnimationName);
        secondCombatAttack = Animator.StringToHash(PlayerRedStats.CombatAttack2AnimationName);
        thirdCombatAttack = Animator.StringToHash(PlayerRedStats.CombatAttack3AnimationName);
        StateContext.CombatRedAnimationsHash = new int[] { firstCombatAttack, secondCombatAttack, thirdCombatAttack };
    }
    public bool ChangeAnimation(int animationHash, bool forced = false)
    {

        if (StateContext.IsDamaged)
            return false;


        // Some animations can be applied only if specific conditions are met
        if (_currentAnimationHash == StateContext.FallAnimationHash && animationHash == StateContext.IdleAnimationHash)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.FallAnimationHash)
                return false;
        }
        else if (_currentAnimationHash == StateContext.FallRedAnimationHash && animationHash == StateContext.IdleRedAnimationHash)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.FallRedAnimationHash)
                return false;
        }
        else if (_currentAnimationHash == StateContext.BlackToRedAnimationHash && animationHash == StateContext.IdleRedAnimationHash)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.BlackToRedAnimationHash)
                return false;
        }
        else if (_currentAnimationHash == StateContext.RedToBlackAnimationHash && animationHash == StateContext.IdleAnimationHash)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.RedToBlackAnimationHash)
                return false;
        }

        if (_currentAnimationHash == animationHash)
        {
            if (forced)
            {
                _animator.Play(animationHash, 0, 0f);
                return true;
            }
            return false;
        }
        else
        {
            _animator.Play(animationHash, 0, 0f);
            _currentAnimationHash = animationHash;
            return true;
        }
    }
}
