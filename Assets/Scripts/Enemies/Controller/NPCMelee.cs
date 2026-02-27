using UnityEngine;
using System.Collections;

public class NPCMelee : NPCClassic, IEnemyAttack
{

    [SerializeField] public float attackRange = 0.5f;
    [SerializeField] public float attackWaitTime = 0.4f;
    [SerializeField] public float attackDelay = 1.5f;
    [SerializeField] public float strikeGap = 0.2f; // Gap between punches
    [SerializeField] public int damage = 20;

    [SerializeField] public GameObject attackIndicatorPrefab;

    private string isAttackingTrigger = "isAttacking";
    private bool isAttacking = false;
    
    private PlayerController currentTarget;
    private int strikeCount = 0;
    private int maxStrikes = 2;
    
    private Coroutine attackCoroutine;
    private Coroutine nextStrikeCoroutine;


    private void Start()
    {
        type = EnemyType.MELEE;
        ChangeState(new FSMPatrol(this, animator));
    }

    protected override void Update()
    {
        base.Update();
    }


    public void Attack(PlayerController player)
    {
        if (!isDead && !isAttacking)
        {
            attackCoroutine = StartCoroutine(PerformAttack(player));
        }
    }
    
    /// <summary>
    /// Call this to cancel the current attack combo.
    /// Used when player escapes mid-combo.
    /// </summary>
    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        
        if (nextStrikeCoroutine != null)
        {
            StopCoroutine(nextStrikeCoroutine);
            nextStrikeCoroutine = null;
        }
        
        isAttacking = false;
        strikeCount = 0;
        currentTarget = null;
        animator.ResetTrigger(isAttackingTrigger);
    }
    
    /// <summary>
    /// Check if currently in an attack combo.
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }

    private IEnumerator PerformAttack(PlayerController player)
    {
        isAttacking = true;
        currentTarget = player;
        strikeCount = 0;

        // Show indicator ONCE before the combo
        ShowAttackIndicator();

        // Wait for indicator duration (player warning)
        yield return new WaitForSeconds(attackWaitTime);

        // Start first attack animation
        animator.SetTrigger(isAttackingTrigger);
        
        // Wait for combo to complete
        yield return new WaitUntil(() => !isAttacking || isDead);
        
        // Cleanup
        currentTarget = null;
        attackCoroutine = null;
    }
    
    private void ShowAttackIndicator()
    {
        if (attackIndicatorPrefab == null) return;
        
        GameObject indicator = Instantiate(attackIndicatorPrefab, transform.position, transform.rotation);
        AttackIndicator ai = indicator.GetComponent<AttackIndicator>();
        if (ai != null)
        {
            ai.Init(attackWaitTime);
        }
    }

    // Called by Animation Event at the damage frame (~0.43s)
    public void DealDamage()
    {
        if (currentTarget == null || isDead) return;
        
        if (GetHorizontalDistance(currentTarget.transform.position) <= attackRange)
        {
            currentTarget.TakeDamage(damage, type, gameObject);
        }
        
        strikeCount++;
    }

    // Called by Animation Event at the END of each attack animation
    public void OnAttackAnimationEnd()
    {
        if (isDead)
        {
            isAttacking = false;
            return;
        }
        
        if (strikeCount < maxStrikes)
        {
            // Wait for gap then trigger next punch (no indicator)
            nextStrikeCoroutine = StartCoroutine(PrepareNextStrike());
        }
        else
        {
            // Combo finished
            isAttacking = false;
            strikeCount = 0;
            animator.ResetTrigger(isAttackingTrigger);
        }
    }
    
    private IEnumerator PrepareNextStrike()
    {
        // Wait for gap between punches (no indicator)
        yield return new WaitForSeconds(strikeGap);
        
        if (!isDead && isAttacking)
        {
            // Trigger next attack in combo
            animator.SetTrigger(isAttackingTrigger);
        }
        
        nextStrikeCoroutine = null;
    }
    
    public float GetAttackRange()
    {
        return attackRange;
    }
    
    public float GetAttackDelay()
    {
        return attackDelay;
    }

    private float GetHorizontalDistance(Vector3 targetPosition)
    {
        return Mathf.Abs(transform.position.x - targetPosition.x);
    }
}
