using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/Input/InputReader")]

/// <summary>
/// Reads player input and raises corresponding events.
/// </summary>
public class InputReader : ScriptableObject, PlayerActionsScript.IPlayerActionsActions, PlayerActionsScript.IUIActions
{
    public event UnityAction<Vector2> Move;
    public event UnityAction<InputActionPhase> Dash;
    public event UnityAction<InputActionPhase> Shoot;
    public event UnityAction<InputActionPhase> NextWeapon;
    public event UnityAction<InputActionPhase> PauseMenu;

    private PlayerActionsScript playerActions;

    public Vector2 movementDirection => playerActions.PlayerActions.Move.ReadValue<Vector2>();
    private void OnEnable()
    {
        if (playerActions == null)
        {
            playerActions = new PlayerActionsScript();
            playerActions.PlayerActions.SetCallbacks(this);
            playerActions.UI.SetCallbacks(this);
        }
    }

    public void EnableUI()
    {
        playerActions.UI.Enable();
    }
    public void DisableUI()
    {
        playerActions.UI.Disable();
    }
    public void EnablePlayerActions()
    {
        playerActions.PlayerActions.Enable();
    }

    public void DisablePlayerActions()
    {
        playerActions.PlayerActions.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move?.Invoke(context.ReadValue<Vector2>());
    }


    public void OnDash(InputAction.CallbackContext context)
    {
        Dash?.Invoke(context.phase);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        Shoot?.Invoke(context.phase);
    }

    public void OnNextWeapon(InputAction.CallbackContext context){
        NextWeapon?.Invoke(context.phase);
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        PauseMenu?.Invoke(context.phase);
    }
}
