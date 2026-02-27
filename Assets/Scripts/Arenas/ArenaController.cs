using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls arena behaviour: starts the arena when the player enters the trigger,
/// spawns waves of enemies, manages arena lasers, and ends the arena when all waves are cleared.
/// </summary>
public class ArenaController : MonoBehaviour
{
    [SerializeField] private Lasers[] lasersOnArena;      // Laser barriers to enable/disable when arena starts/ends
    [SerializeField] private GameObject spawnPoint;       // Position where waves are instantiated
    [SerializeField] private GameObject[] waves;          // Prefabs for waves in order
    [SerializeField] private bool holdActive = false;    

    int currentWave = 0;                                  // Index of the current wave
    bool arenaActive = false;                             // Whether the arena sequence is active

    GameObject currentWaveGameObject;                     // Instance of the currently spawned wave

    private void OnEnable()
    {
        // If the arena was previously active, remove the previous wave instance
        if (arenaActive)
        {
            Destroy(currentWaveGameObject);
        }

        // Reset state to initial values
        currentWave = 0;
        arenaActive = false;

        // Ensure all lasers are turned off at reset (no animations)
        foreach (var laser in lasersOnArena)
        {
            laser.TurnOff(false);
        }
    }

    private void Update()
    {
        // Do nothing if arena isn't running or it's on hold
        if (!arenaActive || holdActive)
            return;

        // When the current wave GameObject has no children, we consider it cleared.
        // Note: currentWaveGameObject is expected to be non-null while arenaActive is true.
        if (currentWaveGameObject.transform.childCount == 0)
        {
            Destroy(currentWaveGameObject);
            currentWave++;
            SpawnWave(currentWave);
        }
    }

    /// <summary>
    /// Spawns a wave by index. If index equals the number of waves, the arena ends.
    /// </summary>
    /// <param name="waveId">Zero-based wave index to spawn.</param>
    private void SpawnWave(int waveId)
    {
        // All waves completed -> end arena
        if (waveId == waves.Length)
        {
            EndArena();
        }
        else if (waveId < waves.Length)
        {
            // Instantiate the requested wave at the configured spawn point
            currentWaveGameObject = Instantiate(waves[waveId], spawnPoint.transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Ends the arena: turn off lasers and mark arena inactive.
    /// </summary>
    private void EndArena()
    {
        foreach (var laser in lasersOnArena)
        {
            laser.TurnOff();
        }
        arenaActive = false;
    }

    /// <summary>
    /// Trigger handler: when the player enters the arena trigger, heal the player if necessary,
    /// activate lasers and begin spawning waves unless the arena is already active or on hold.
    /// </summary>
    /// <param name="collision">Collider2D that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prevent re-triggering or starting when on hold or when all waves already completed
        if (arenaActive || currentWave == waves.Length || holdActive)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();

            // Ensure the player has at least 150 HP when the arena starts
            if (pc.StateContext.CurrentHealth < 150)
            {
                pc.Heal(150 - pc.StateContext.CurrentHealth);
            }

            // Turn on all arena lasers to block exit/entry
            foreach (var laser in lasersOnArena)
            {
                laser.TurnOn();
            }

            // Mark arena as active and spawn the first (or current) wave
            arenaActive = true;
            SpawnWave(currentWave);
        }
    }


    public bool IsArenaActive()
    {
        return arenaActive;
    }

    public void Activate()
    {
        if (holdActive)
            arenaActive = true;
    }
}
