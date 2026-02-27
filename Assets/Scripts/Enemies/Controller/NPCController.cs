using UnityEngine;

public class NPCController : MonoBehaviour
{
    public FSMState currentState;
    protected EnemyType type;

    protected virtual void Update()
    {
        currentState?.Update();
    }

    protected virtual void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
    protected virtual void OnStateChange(FSMState oldState, FSMState newState) { }

    public void ChangeState(FSMState newState)
    {
        // Added if check to avoid unnecessary state changes
        if (currentState == newState || newState == null) return;   // No change
        //Debug.Log($"State Change: {currentState?.GetType().Name ?? "null"} -> {newState?.GetType().Name ?? "null"}");
        FSMState oldState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
        OnStateChange(oldState, newState);
        //Debug.Log("Changed state to " + newState.GetType().Name);
    }

    public void StopState()
    {
        ChangeState(null);
    }

    // Placed here for convenience (Ranged enemies also need these functions), could be moved to a utility class later
    public Collider2D[] GetPlayersNearby(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        return System.Array.FindAll(hits, c => c.CompareTag("Player"));
    }

    // Placed here for convenience (Ranged enemies also need these functions), could be moved to a utility class later
    public PlayerController FindPlayer(float radius)
    {
        Collider2D[] playerHits = GetPlayersNearby(radius);
        //Debug.Log($"Found {playerHits.Length} players within {radius} units.");
        if (playerHits.Length > 0)
        {
            foreach (Collider2D c in playerHits)
            {
                PlayerController pc = c.GetComponent<PlayerController>();
                if (pc != null)
                {
                    return pc;
                }
            }
        }
        return null;
    }

    public EnemyType GetEnemyType()
    {
        return type;
    }
}
