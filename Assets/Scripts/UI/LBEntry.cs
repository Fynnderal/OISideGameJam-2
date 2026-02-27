using UnityEngine;
using UnityEngine.UI;
using System;

// Setter for the Leaderboard prefabs
public class LBEntry : MonoBehaviour
{

    [SerializeField] private Text rankt;
    [SerializeField] private Text namet;
    [SerializeField] private Text timet;
    [SerializeField] private Text deathst;
    private SaveData saveData;
    private MenuController mc;

    public void Set(MenuController mc, SaveData sd, int rank)
    {
        this.mc = mc;
        saveData = sd;
        rankt.text = "#" + (rank + 1).ToString();
        namet.text = sd.name;
        TimeSpan ts = TimeSpan.FromSeconds(sd.playtime);
        string time = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        timet.text = time;
        if (deathst != null) deathst.text = sd.deaths.ToString();
    }

    public void OnDelete()
    {
        SettingsManager.globalLeaderboard.saves.Remove(saveData);
        SettingsManager.saveLeaderboard();
        mc.FillLeaderboard();
    }
}
