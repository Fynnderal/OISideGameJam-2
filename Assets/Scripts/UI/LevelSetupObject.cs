using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public class LevelSetupObject : MonoBehaviour
{

    [SerializeField]
    private List<Checkpoint> checkpoints;
    [SerializeField]
    public GameObject player;
    [SerializeField] private InGameController inGameController;

    public void Awake()
    {
        LoadFromSave();
    }

    public void Start()
    {
        player.GetComponent<PlayerController>().SetHealth(SettingsManager.globalSave.health);

    }

    void Update()
    {
        SettingsManager.IncTime(Time.deltaTime);
        if (inGameController != null) inGameController.SetTimer(SettingsManager.globalSave.playtime);
        else Debug.LogWarning("lso has no reference to inGameController!");
    }

    // load checkpoint from save and set chunks
    public void LoadFromSave()
    {
        SaveData saveData = SettingsManager.globalSave;
        if (saveData != null)
        {
            if(saveData.checkpoint >=0 && saveData.checkpoint < checkpoints.Count)
            {
                for (int i = 0; i < checkpoints.Count; i++)
                {
                    checkpoints[i].lso = this;
                    if (i == saveData.checkpoint || i == saveData.checkpoint + 1)
                    {
                        checkpoints[i].SetChunks(true);
                    }
                    else
                    {
                        checkpoints[i].SetChunks(false);
                    }
                }
                SetCheckpoint(checkpoints[saveData.checkpoint], true);
                Spawn();
                Debug.Log("checkpoint " + saveData.checkpoint + " loaded");
            }
            else
            {
                Debug.Log("invalid cp " + saveData.checkpoint + ": not set");
            }

        }
        else
        {
            Debug.Log("INVALID SAVE ERROR");
        }
    }

    private Checkpoint active_cp;
    // Set active checkpoint and add to save
    public void SetCheckpoint(Checkpoint cp, bool load = false)
    {
        int index = checkpoints.IndexOf(cp);
        if (index == -1 )
        {
            Debug.Log("invalid cp: not set");
            return;
        }
        if (index <= checkpoints.IndexOf(active_cp))
        {
            if (index >= 0 && index < checkpoints.Count)
            {
                checkpoints[index].SetChunks(false, false);
                checkpoints[index].SetChunks(true, false);
            }
            return;
        }
        active_cp = cp;

        SettingsManager.globalSave.checkpoint = index;

        if (!load)
        {
            if (index != 0 && player.GetComponent<PlayerController>().StateContext.CurrentHealth > 0)
                SettingsManager.globalSave.health = player.GetComponent<PlayerController>().StateContext.CurrentHealth;
            else
                SettingsManager.globalSave.health = player.GetComponent<PlayerController>().PlayerStatsBlack.MaxHealth;
        }

        if (index - 1 >= 0 && index - 1 < checkpoints.Count) checkpoints[index - 1].SetChunks(false);
        if (index >= 0 && index < checkpoints.Count)
        {
            checkpoints[index].SetChunks(false, false);
            checkpoints[index].SetChunks(true, false);
        }
        if (index + 1 >= 0 && index + 1 < checkpoints.Count) checkpoints[index + 1].SetChunks(true);
        SettingsManager.saveSave();
    }

    // Respawn player at checkpoint
    public void Respawn()
    {
        active_cp.Respawn(player);
    }

    // Spawn player at checkpoint
    public void Spawn()
    {
        active_cp.Spawn(player);
    }

    // Load to next level
    public void LoadNextLevel()
    {
        SettingsManager.globalSave.level += 1;
        SettingsManager.globalSave.checkpoint = 0;
        SettingsManager.globalSave.health = player.GetComponent<PlayerController>().PlayerStatsBlack.MaxHealth;
        SettingsManager.saveSave();
        SettingsManager.loadLevel();
    }
}

