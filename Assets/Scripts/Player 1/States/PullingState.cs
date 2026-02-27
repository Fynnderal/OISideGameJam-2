using UnityEngine;

/// <summary>
/// Implements the state where the player is being pulled by a hook to the wall
public class PullingState : PlayerBaseState
{
    private readonly PlayerRedStats _playerRedStats;

    public PullingState(PlayerController playerController, PlayerRedStats playerRedStats, StateContext stateContext) 
        : base(playerController, playerRedStats, stateContext)
    {
        _playerRedStats = playerRedStats;
    }

    public override void OnEnter()
    {
        _playerController.RB.gravityScale = 0f;
        _playerController.RB.linearVelocity = Vector2.zero;
        _playerController.AnimationController.ChangeAnimation(_stateContext.PullingAnimationHash);
    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
        _playerController.RB.AddForce(Vector2.right * _stateContext.PullingDirection * _playerRedStats.HookForce);
    }

    public override void LateUpdate() {
        _playerController.LineRenderer.SetPosition(0, _playerController.Hook.position);
    }

    public override void OnExit()
    {
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;
        _stateContext.IsPulling = false;
        _playerController.LineRenderer.positionCount = 0;
        _stateContext.JumpCount = _playerRedStats.JumpsAllowed;
    }

}
