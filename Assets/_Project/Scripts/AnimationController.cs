using UnityEngine;

public class AnimationController
{
    Animator _animator;

    private int _currentAnimationHash;
    public int CurrentAnimationHash => _currentAnimationHash;
    public AnimationController(Animator animator)
    {
        _animator = animator;
        _currentAnimationHash = -1;
    }

    public bool ChangeAnimation(int animationHash, bool forced = false)
    {

        //if (StateContext.IsDamaged)
        //    return false;


        //// Some animations can be applied only if specific conditions are met
        //if (_currentAnimationHash == StateContext.FallAnimationHash && animationHash == StateContext.IdleAnimationHash)
        //{
        //    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.FallAnimationHash)
        //        return false;
        //}
        //else if (_currentAnimationHash == StateContext.FallRedAnimationHash && animationHash == StateContext.IdleRedAnimationHash)
        //{
        //    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.FallRedAnimationHash)
        //        return false;
        //}
        //else if (_currentAnimationHash == StateContext.BlackToRedAnimationHash && animationHash == StateContext.IdleRedAnimationHash)
        //{
        //    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.BlackToRedAnimationHash)
        //        return false;
        //}
        //else if (_currentAnimationHash == StateContext.RedToBlackAnimationHash && animationHash == StateContext.IdleAnimationHash)
        //{
        //    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != StateContext.RedToBlackAnimationHash)
        //        return false;
        //}

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
