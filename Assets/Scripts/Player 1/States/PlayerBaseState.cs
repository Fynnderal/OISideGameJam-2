using UnityEngine;

public abstract class PlayerBaseState : BaseState
{
    protected readonly PlayerController _playerController;
    protected readonly PlayerBaseStats _playerStats;
    protected readonly StateContext _stateContext;

    protected PlayerBaseState(PlayerController playerController, PlayerBaseStats playerStats, StateContext stateContext) : base()
    {
        this._playerController = playerController;
        this._playerStats = playerStats;
        this._stateContext = stateContext;
    }
}
