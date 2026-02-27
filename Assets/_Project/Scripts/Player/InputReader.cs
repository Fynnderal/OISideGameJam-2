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
    public event UnityAction<InputActionPhase> Jump;
    public event UnityAction<InputActionPhase> Dash;
    public event UnityAction<InputActionPhase> MeleeAttack;
    public event UnityAction<InputActionPhase> GlideOrHook;
    //public event UnityAction<InputActionPhase> Ranged; 
    public event UnityAction<InputActionPhase> SuitChange;
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

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump?.Invoke(context.phase);

    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Dash?.Invoke(context.phase);
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        MeleeAttack?.Invoke(context.phase);
    } 

    public void OnGlidingHook(InputAction.CallbackContext context)
    {
        GlideOrHook?.Invoke(context.phase);
    }

    //public void OnRangedAttack(InputAction.CallbackContext context)
    //{
    //    Ranged?.Invoke(context.phase);  
    //}

    public void OnSuitChange(InputAction.CallbackContext context)
    {
        SuitChange?.Invoke(context.phase);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        PauseMenu?.Invoke(context.phase);
    }
}
