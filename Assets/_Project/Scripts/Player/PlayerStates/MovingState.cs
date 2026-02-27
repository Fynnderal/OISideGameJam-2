using UnityEngine;

public class MovingState : BaseState
{
    protected readonly PlayerController _playerController;
    protected readonly Stats _playerStats;
    protected readonly StateContextNew _stateContext;

    protected MovingState(PlayerController playerController, Stats playerStats, StateContextNew stateContext) : base()
    {
        this._playerController = playerController;
        this._playerStats = playerStats;
        this._stateContext = stateContext;
    }

    public override void OnEnter()
    {

    }

    public override void Update()
    {
    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnExit()
    {

    }

    protected void HandleMovement(float acceleration, float deceleration, float speed, float accelerationPow)
    {
        float targetSpeed = _playerController.Input.movementDirection.x * speed;
        float speedDifference = Mathf.Max(Mathf.Abs(targetSpeed - _playerController.RB.linearVelocityX), 0.01f);

        float finalAcceleration = _playerController.Input.movementDirection.x != 0 ? acceleration : deceleration;
        _playerController.RB.linearVelocityX = Mathf.MoveTowards(_playerController.RB.linearVelocityX, targetSpeed, Mathf.Pow(speedDifference, accelerationPow) * finalAcceleration * Time.fixedDeltaTime);

    }

}
