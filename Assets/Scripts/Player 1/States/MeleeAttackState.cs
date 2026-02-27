using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;


/// <summary>
/// Implements attack
/// </summary>
public class MeleeAttackState : MovementAndJumpState
{

    public MeleeAttackState(PlayerController playerController, PlayerStatsBlack playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _stateContext.currentCombatId = 0;
        _playerController._playingAttackAnimation = false;
    }

    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnExit()
    {
        base.OnExit();
        if (_stateContext.IsBlack)
            _playerController.AnimationController.ChangeAnimation(_stateContext.IdleAnimationHash);
        else
            _playerController.AnimationController.ChangeAnimation(_stateContext.IdleRedAnimationHash);

        _stateContext.CombatBufferTimer.Stop();
        _playerController._playingAttackAnimation = false;
        _stateContext.currentCombatId = 0; 
    }

    protected override void CheckAnimations()
    {
        if (_stateContext.CombatBufferTimer.isRunning && !_playerController._playingAttackAnimation)
        {
            if (_stateContext.IsBlack)
                _playerController.AnimationController.ChangeAnimation(_stateContext.CombatAnimationsHash[_stateContext.currentCombatId]);
            else
                _playerController.AnimationController.ChangeAnimation(_stateContext.CombatRedAnimationsHash[_stateContext.currentCombatId]);

            IncrementCombatId();

            return;
        }



        if (_playerController._playingAttackAnimation)
            return;

        if (!_stateContext.CombatBufferTimer.isRunning)
        {
            _stateContext.currentCombatId = 0;
        }

        base.CheckAnimations();
    }

    // Increments the combat ID for combo attacks
    private void IncrementCombatId()
    {
        _stateContext.currentCombatId = (_stateContext.currentCombatId + 1) % 3;
    }
}
