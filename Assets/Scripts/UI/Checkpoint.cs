using UnityEngine;
using System.Collections.Generic;


public class Checkpoint : MonoBehaviour
{

    [SerializeField]
    private GameObject chunkholder;

    [SerializeField]
    private Animator animator;

    public LevelSetupObject lso { private get; set; }
    [SerializeField]
    private Transform savepos;

    private GameObject active_cp;

    // Respawn player and spawners, inc death
    public void Respawn(GameObject player)
    {
        player.transform.position = savepos.position;
        player.GetComponent<PlayerController>().StateContext.CurrentHealth = SettingsManager.globalSave.health;
        SettingsManager.IncDeath(1);
        RespawnEnemy();
        PlaySpawnAnim();
    }
    // Respawn player and spawners
    public void Spawn(GameObject player)
    {
        player.transform.position = savepos.position;

        RespawnEnemy();
        PlaySpawnAnim();
    }

    // Enable/disable chunks in chunkholder
    public void SetChunks(bool load, bool respawn = true)
    { 
        foreach (Transform child in chunkholder.transform)
        {
            child.gameObject.SetActive(load);
        }
        if(load && respawn) RespawnEnemy();
    }

    // Play spawn animation
    public void PlaySpawnAnim()
    {
        animator.Play("press_checkpoint_Clip", 0, 0f);
        Debug.Log("Playing cp clip");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(lso != null)
        {
            if (other.gameObject != lso.player) return;
            PlaySpawnAnim();
            lso.SetCheckpoint(this);
        }
    }

    // enemies prefab for spawning
    [SerializeField]
    private GameObject enemyPrefab;
    // position to spawn enemies at
    [SerializeField]
    private Transform enemySpawnPos;
    private GameObject enemyInstance = null;

    // Spawn enemy prefab
    public void RespawnEnemy()
    {
        if(enemySpawnPos == null || enemyPrefab == null)
        {
            Debug.Log("Error: enemy spawn point not set");
            return;
        }

        DespawnEnemy();
        // spawn new enemy
        GameObject enemy = Instantiate(
            enemyPrefab,
            enemySpawnPos.position,
            Quaternion.identity
        );

        enemyInstance = enemy;
    }

    // Destroy enemy prefab instance
    public void DespawnEnemy()
    {
        if (enemyInstance != null) Destroy(enemyInstance);
    }
}

