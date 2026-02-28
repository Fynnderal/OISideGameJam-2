using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class UniversalState : MovingState
{
    
    public UniversalState(PlayerControllerTopDown playerController, Stats playerStats, StateContextNew stateContext) : base(playerController, playerStats, stateContext)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void Update()
    {
        base.Update();

        Aim();

    }

    private void Aim()
    {
        Vector3 mousePosition = (Vector2)_playerController.CameraMain.ScreenToWorldPoint(Mouse.current.position.ReadValue());


        float angle = Mathf.Atan2(mousePosition.y - _playerController.Transform.position.y, mousePosition.x - _playerController.Transform.position.x) * Mathf.Rad2Deg;
        _playerController.WeaponAxis.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Vector3 localScale = Vector3.one;


        if (angle > 90 || angle < -90)
        {
            _playerController.SpriteRenderer.flipX = false;

            localScale.y = -1;
        }
        else
        {
            _playerController.SpriteRenderer.flipX = true;
            localScale.y = 1;
        }
        _playerController.WeaponAxis.transform.localScale = localScale;
        //_playerController.Transform.localScale = localScale;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
