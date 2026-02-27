using UnityEngine;


/// <summary>
/// Implements the swing state for the player when attached to a anchor.
/// </summary>
public class SwingState : PlayerBaseState
{
    protected PlayerRedStats _playerStatsRed;
    public SwingState(PlayerController playerController, PlayerRedStats playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext)
    {
        _playerStatsRed = playerStats;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _playerController.RB.gravityScale = _stateContext.GravityScaleCached;
        _stateContext.JumpCount = _playerStats.JumpsAllowed - 1;
        _stateContext.currentAnchor = _stateContext.nearestAnchor;
        _playerController.AnimationController.ChangeAnimation(_stateContext.HookSwingAnimationHash);
        AttachHook(_stateContext.currentAnchor.transform.position);
    }

    public override void Update()
    {
        base.Update();
    

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerController.RB.AddForce(Vector2.right * _playerController.Input.movementDirection.x * _playerStatsRed.SwingForce);
        _playerController.RB.linearVelocityX = Mathf.Clamp(_playerController.RB.linearVelocityX, -_playerStatsRed.MaxSwingSpeed, _playerStatsRed.MaxSwingSpeed);
        _playerController.RB.linearVelocityY = Mathf.Clamp(_playerController.RB.linearVelocityY, -_playerStatsRed.MaxSwingSpeed, _playerStatsRed.MaxSwingSpeed);

    }

    public override void LateUpdate()
    {
        DrawLine(_stateContext.currentAnchor.transform.position);
    }
    public override void OnExit()
    {
        base.OnExit();
        _stateContext.IsSwinging = false;
        _playerController.SpringJoint.enabled = false;
        _playerController.LineRenderer.positionCount = 0;
        _stateContext.currentAnchor = null;
    }
    // Attaches the grappling hook to the anchor point
    private void AttachHook(Vector2 anchorPosition)
    {
        _playerController.SpringJoint.connectedAnchor = anchorPosition;
        _playerController.SpringJoint.autoConfigureDistance = false;
        _playerController.SpringJoint.distance = _playerStatsRed.Distance;
        _playerController.SpringJoint.frequency = _playerStatsRed.Frequency;
        _playerController.SpringJoint.dampingRatio = _playerStatsRed.DampingRatio;
        _playerController.SpringJoint.enabled = true;
        _stateContext.IsSwinging = true;
    }

    // Draws grappling hook line
    private void DrawLine(Vector2 anchorPosition)
    {
        _playerController.LineRenderer.positionCount = 2;
        _playerController.LineRenderer.SetPosition(0, anchorPosition);
        _playerController.LineRenderer.SetPosition(1, _playerController.Hook.position);
    }
}
