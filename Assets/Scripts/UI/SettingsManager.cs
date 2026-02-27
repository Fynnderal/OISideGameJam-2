using UnityEngine;
using UnityEngine.SceneManagement;

// class for handling save and settings globally. Singleton
public static class SettingsManager
{
    
    public static Settings globalSettings = null;
    private const string settingsName = "settings.json";

    // active loaded save holder
    public static SaveData globalSave = null;
    private const string saveName = "save.json";

    // active loaded leaderboard holder
    public static Leaderboard globalLeaderboard = null;
    private const string leaderboardName = "leaderboard.json";

    // load save immediately on start
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (!loadSettings())
        {
            globalSettings = new Settings(1920, 1080, 60, 1, FullScreenMode.FullScreenWindow);
        }
        

        loadSave();
        if (!loadLeaderboard())
        {
            globalLeaderboard = new Leaderboard();
        }
        Debug.Log("Settings Loaded");
    }

    // load settings from file
    public static bool loadSettings(string fname = settingsName)
    {
        globalSettings = JsonManager.Load<Settings>(fname);
        return globalSettings != null;
    }

    // save settings to file
    public static bool saveSettings(string fname = settingsName)
    {
        return JsonManager.Save<Settings>(fname, globalSettings);
    }

    // load leaderboard from file
    public static bool loadLeaderboard(string fname = leaderboardName)
    {
        globalLeaderboard = JsonManager.Load<Leaderboard>(fname);
        return globalLeaderboard != null;
    }

    // save leaderboard to file
    public static bool saveLeaderboard(string fname = leaderboardName)
    {
        return JsonManager.Save<Leaderboard>(fname, globalLeaderboard);
    }

    // load active save from file
    public static bool loadSave(string fname = saveName)
    {
        globalSave = JsonManager.Load<SaveData>(fname);
        return globalSettings != null;
    }

    // save active save to file
    public static bool saveSave(string fname = saveName)
    {
        return JsonManager.Save<SaveData>(fname, globalSave);
    }

    // delete save file
    public static bool deleteSave(string fname = saveName)
    {
        return JsonManager.Delete(fname);
    }

    // load level from active save
    public static void loadLevel()
    {
        Debug.Log($"Loading save: level {globalSave.level}, cp {globalSave.checkpoint}, deaths {globalSave.deaths}, health {globalSave.health}");
        SceneManager.LoadScene(globalSave.level + "level");
    }

    // add deaths to active save
    public static void IncDeath(int inc)
    {
        globalSave.deaths += inc;
        saveSave();
    }

    // increase playtime in active save
    public static void IncTime(double dtime)
    {
        globalSave.playtime += dtime;
    }

    // finish active save
    public static void FinishGame()
    {
        globalSave.finished = true;
        SceneManager.LoadScene("Menu");
    }

    // Apply screen globalSettings to engine
    public static void ApplyScreenSettings()
    {
        if (globalSettings != null)
        {
            Screen.SetResolution(
                globalSettings.width,
                globalSettings.height,
                globalSettings.windowMode,
                new RefreshRate { numerator = globalSettings.refreshRateNum, denominator = globalSettings.refreshRateDen }
            );
        }
    }

    // Apply audio globalSettings to engine
    public static void ApplyAudioSettings()
    {
        if (globalSettings != null)
        {
            AudioManager.SetMasterVolume(globalSettings.masterVolume);
            AudioManager.SetSFXVolume(globalSettings.sfxVolume);
            AudioManager.SetMusicVolume(globalSettings.musicVolume);
        }
    }

}

