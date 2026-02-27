using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

/// <summary>
/// Handles player input and maps it to player actions.
/// </summary>
public class InputHandler : MonoBehaviour
{
    [SerializeField] PlayerController _player;
    [SerializeField] InGameController _inGameController;

    StateContext StateContext => _player.StateContext;
    InputReader Input => _player.Input;

    private void Start()
    {
        _player.Input.EnablePlayerActions();
        _player.Input.EnableUI();
    }
    private void OnEnable()
    {
        _player.Input.EnablePlayerActions();
        _player.Input.EnableUI();

        _player.Input.Jump += OnJump;
        _player.Input.Dash += OnDash;
        _player.Input.MeleeAttack += OnMeleeAttack;
        _player.Input.GlideOrHook += OnGlideOrHook;
        //_player.Input.Ranged += OnRangedAttack;
        _player.Input.SuitChange += OnSuitChange;
        _player.Input.PauseMenu += OnPause;

    }

    private void OnDisable()
    {
        _player.Input.Jump -= OnJump;
        _player.Input.PauseMenu -= OnPause;
        _player.Input.Dash -= OnDash;
        _player.Input.MeleeAttack -= OnMeleeAttack;
        _player.Input.GlideOrHook -= OnGlideOrHook;
        //_player.Input.Ranged -= OnRangedAttack;
        _player.Input.SuitChange -= OnSuitChange;

    }
    public void OnPause()
    {
        if (StateContext.InPauseMenu)
        {
            Input.EnablePlayerActions();
            Cursor.visible = false;
            _inGameController.ResumeGame();
            StateContext.InPauseMenu = false;
            _player.AudioListener.enabled = true;   
        }
        else
        {
            Input.DisablePlayerActions();
            Cursor.visible = true;
            _inGameController.PauseGame();
            StateContext.InPauseMenu = true;
            _player.AudioListener.enabled = false;
        }
    }

    public void OnPause(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            OnPause();
        }
    }
        
    private void OnGlideOrHook(InputActionPhase phase)
    {
        if (_player.StateContext.IsBlack)
        {
            OnGlide(phase);
        }
        else
        {
            OnHook(phase);
        }
    }

    private void OnHook(InputActionPhase phase)
    {
        if (_player.StateContext.SideDashTimer.isRunning || _player.StateContext.DownDashTimer.isRunning)
            return;

        if (phase != InputActionPhase.Started)
            return;

        if (!_player.SpringJoint.enabled && _player.StateContext.nearestAnchor == null)
        {
            if (_player.StateContext.IsTouchingWall == 0)
                SideHook();
            return;
        }

        if (_player.SpringJoint.enabled)
            _player.StateContext.IsSwinging = false;
        else
            _player.StateContext.IsSwinging = true;
    }

    private void OnSuitChange(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            _player.StateContext.IsBlack = !_player.StateContext.IsBlack;

            if (!_player.StateContext.IsBlack)
            {
                _player.AnimationController.ChangeAnimation(_player.StateContext.BlackToRedAnimationHash);
                _player.ChangeFromBlackParticles.Play();
            }
            else
            {
                _player.AnimationController.ChangeAnimation(_player.StateContext.RedToBlackAnimationHash);
                _player.ChangeFromRedParticles.Play();
            }

            _player.PlayerSounds.PlaySuitChangeSound();

        }
    }

    private void OnJump(InputActionPhase phase)
    {
        switch (phase)
        {
            case InputActionPhase.Started:
                StateContext.JumpBufferTimer.Start();
                _player._playingAttackAnimation = false;
                StateContext.CombatBufferTimer.Stop();
                StateContext.JumpCut = false;
                StateContext.JumpWasHeld = false;
                break;

            case InputActionPhase.Canceled:
                if (StateContext.JumpBufferTimer.isRunning || !StateContext.JumpWasHeld)
                    StateContext.JumpCut = true;

                break;

            case InputActionPhase.Performed:
                StateContext.JumpWasHeld = true;
                StateContext.JumpCut = false;
                break;
        }
    }


    private void OnMeleeAttack(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started && !StateContext.SideDashTimer.isRunning && !StateContext.DownDashTimer.isRunning && !StateContext.IsDamaged)
        {
            StateContext.JumpBufferTimer.Stop();

            StateContext.CombatBufferTimer.Start();
        }
    }

    private void OnDash(InputActionPhase phase)
    {
        if (Input.movementDirection.y >= 0 && phase == InputActionPhase.Started && !StateContext.SideDashTimer.isRunning)
            StateContext.SideDashTimer.Start();
        else if (Input.movementDirection.y < 0 && phase == InputActionPhase.Started && !StateContext.DownDashTimer.isRunning)
            StateContext.DownDashTimer.Start();
    }

    private void OnGlide(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started && !StateContext.SideDashTimer.isRunning && !StateContext.DownDashTimer.isRunning)
            StateContext.IsGliding = true;
        else if (phase == InputActionPhase.Canceled)
            StateContext.IsGliding = false;
    }

    public void SideHook()
    {
        RaycastHit2D hit = Physics2D.Raycast(_player.PlayerCenter.position, _player.IsFacingRight ? Vector2.right : Vector2.left, _player.PlayerRedStats.HookMaxDistance, ~(_player.PlayerLayer | _player.TriggerLayer));
        if (hit && (_player.PlayerStatsBlack.WallLayer & (1 << hit.collider.gameObject.layer)) != 0)
        {
            StateContext.PullingDirection = _player.IsFacingRight ? 1 : -1;
            StateContext.IsPulling = true;
            _player.LineRenderer.positionCount = 2;
            _player.LineRenderer.SetPosition(0, _player.PlayerCenter.position);
            _player.LineRenderer.SetPosition(1, hit.point);
        }
    }
    bool _isInHitStop = false;

    public void StartHitStop(float duration)
    {
        StartCoroutine(HitStop(duration));
    }

    /// <summary>
    /// Hit stop for a critical combat attack
    /// </summary>
    /// <param name="duration">Duration of the hit stop</param>
    /// <returns></returns>
    public IEnumerator HitStop(float duration)
    {
        if (_isInHitStop) yield break;

        _isInHitStop = true;
        _player.Input.DisablePlayerActions();
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        _isInHitStop = false;
        _player.Input.EnablePlayerActions();

    }

    public void CriticalDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(new Vector3(transform.position.x + Mathf.Sign(transform.localScale.x) * _player.PlayerStatsBlack.AttackCenterRelativeToPlayer.x, transform.position.y + _player.PlayerStatsBlack.AttackCenterRelativeToPlayer.y, 0f), _player.PlayerStatsBlack.AttackRadius, _player.PlayerStatsBlack.EnemyLayer);
        if (DealDamageAll(enemies, Mathf.FloorToInt(_player.PlayerStatsBlack.AttackDamage * _player.PlayerStatsBlack.MeleeCritMultiplier)))
        {
            StartCoroutine(CriticalHitCoroutine());
        }
    }

    private IEnumerator CriticalHitCoroutine()
    {
        CameraController.Instance.ScreenShake(_player.ImpulseSource, _player.PlayerStatsBlack.MeleeCritCameraShake);
        StartHitStop(_player.PlayerStatsBlack.HitStopDuration);
        yield return null;

    }

    /// <summary>
    /// Deals to all enemies in the array
    /// </summary>
    /// <param name="enemies">array of enemies</param>
    /// <param name="damage">amount of damage to inflict</param>
    /// <returns></returns>
    public bool DealDamageAll(Collider2D[] enemies, int damage)
    {
        bool hitEnemy = false;
        foreach (var enemy in enemies)
        {

            if (enemy.TryGetComponent<IEnemy>(out var enemyScript))
            {
                if (StateContext.IsBlack)
                    enemyScript.TakeDamage(damage, Suit.BLACK, this.gameObject);
                else
                    enemyScript.TakeDamage(damage, Suit.RED, this.gameObject);

                hitEnemy = true;
            }
        }

        return hitEnemy;
    }


    /// <summary>
    /// Called by Animation Event at the damage frame of the melee attack. Finds nearby enemies and deals damage to them.
    /// </summary>
    private void OnMeleeAttackDealingDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(new Vector3(transform.position.x + Mathf.Sign(transform.localScale.x) * _player.PlayerStatsBlack.AttackCenterRelativeToPlayer.x, transform.position.y + _player.PlayerStatsBlack.AttackCenterRelativeToPlayer.y, 0f), _player.PlayerStatsBlack.AttackRadius, _player.PlayerStatsBlack.EnemyLayer);
        DealDamageAll(enemies, _player.PlayerStatsBlack.AttackDamage);
    }


}
