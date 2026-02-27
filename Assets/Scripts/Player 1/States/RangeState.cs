//using Unity.VisualScripting;
//using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

//public class RangeState : PlayerBaseState
//{
//    public RangeState(PlayerController playerController, PlayerStats playerStats, StateContext stateContext) : base(playerController, playerStats, stateContext) {
//        _stateContext.RangeChargingTimer.OnTimerStop += PerformShoot;
//        _stateContext.RangeChargingTimer.OnTimerForcedStop += OnRangeChargingTimerForcedStop;
//    }

//    public override void OnEnter()
//    {
//        Debug.Log("Enter Range State");
//        _playerController.RB.linearVelocity = new Vector2(0f, 0f);
//        //_playerController.CurrentMoveSpeed = 0f;
//        CameraController.Instance.ScreenShake(_playerController.ImpulseSource, _playerStats.ChargingCameraShake);
//    }



//    public override void OnExit()
//    { 
//        _stateContext.RangeChargingTimer.OnTimerStop -= PerformShoot;   
//        _stateContext.RangeChargingTimer.OnTimerForcedStop -= OnRangeChargingTimerForcedStop;
//    }
//    private void OnRangeChargingTimerForcedStop() => CameraController.Instance.StopAllCurrentShakes();
//    private void PerformShoot()
//    {
//        RaycastHit2D[] hitEnemies;

//        if (_playerController.IsFacingRight)
//            hitEnemies = Physics2D.RaycastAll(_playerController.Collider.bounds.center, Vector2.right, _playerStats.PiercingShotMaxRange);
//        else
//            hitEnemies = Physics2D.RaycastAll(_playerController.Collider.bounds.center, Vector2.left, _playerStats.PiercingShotMaxRange);

//        CameraController.Instance.StopAllCurrentShakes();
//        CameraController.Instance.ScreenShake(_playerController.ImpulseSource, _playerStats.PiercingShotCameraShake);

//        float targetXPosition = _playerController.IsFacingRight ? _playerStats.PiercingShotMaxRange : -_playerStats.PiercingShotMaxRange;
//        foreach (var hit in hitEnemies)
//        {
//            if (_playerStats.PlayerLayer.value >> hit.collider.gameObject.layer == 1)
//                continue;

//            if (_playerStats.EnemyLayer.value >> hit.collider.gameObject.layer != 1)
//            {
//                if (_playerController.IsFacingRight)
//                    targetXPosition = hit.collider.bounds.min.x;
//                else
//                    targetXPosition = hit.collider.bounds.max.x;

//                Debug.Log(hit.collider.name);
//                break;
//            }

//            if (hit.collider.TryGetComponent<IEnemy>(out IEnemy enemyScript))
//                enemyScript.TakeDamage(_playerStats.AttackDamage, AttackType.RANGED, Suit.SPADES);
            
//        }

//        _playerController.CreateTracer(targetXPosition);
        

//    }
//}
