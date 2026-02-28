using UnityEngine;

public class MovingState : BaseState
{
    protected readonly PlayerControllerTopDown _playerController;
    protected readonly Stats _playerStats;
    protected readonly StateContextNew _stateContext;

    protected MovingState(PlayerControllerTopDown playerController, Stats playerStats, StateContextNew stateContext) : base()
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
        Move();

    }

    public override void OnExit()
    {

    }


    protected void Move()
    {
        HandleMovement(_playerStats.GroundAcceleration, _playerStats.GroundDeceleration, _playerStats.RunSpeed, _playerStats.GroundAccelerationPow);        
    }
    protected void HandleMovement(float acceleration, float deceleration, float speed, float accelerationPow)
    {
        Vector2 targetSpeed = (_playerController.Input.movementDirection.magnitude > 1 ? _playerController.Input.movementDirection.normalized : _playerController.Input.movementDirection) * speed;
        float speedDifference = Mathf.Max((targetSpeed - _playerController.RB.linearVelocity).magnitude, 0.01f);
        float finalAcceleration = _playerController.Input.movementDirection.magnitude > 0.01f ? acceleration : deceleration;
        _playerController.RB.linearVelocity = Vector2.MoveTowards(_playerController.RB.linearVelocity, targetSpeed, Mathf.Pow(speedDifference, accelerationPow) * finalAcceleration * Time.fixedDeltaTime);

    }

}
