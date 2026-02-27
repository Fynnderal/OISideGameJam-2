using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;

// controlling menu ui
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject lbPanel;
    [SerializeField] private GameObject lbPopup;

    [SerializeField] private InputField inputField;
    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject classicLB;
    [SerializeField] private GameObject classicLBprefab;
    [SerializeField] private GameObject proLB;
    [SerializeField] private GameObject proLBprefab;

    [SerializeField] private PlayableDirector _deathCutScene;

    void Start()
    {
        continueButton.interactable = SettingsManager.globalSave != null;
        ShowMainMenu();
        FillLeaderboard();
        SettingsManager.ApplyAudioSettings();
        if (continueButton.interactable && SettingsManager.globalSave.finished) ShowLBPopup();
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (lbPanel != null) lbPanel.SetActive(false);
        if (lbPopup != null) lbPopup.SetActive(false);
    }

    public void ShowSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (lbPanel != null) lbPanel.SetActive(false);
        if (lbPopup != null) lbPopup.SetActive(false);
    }

    public void ShowLB()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (lbPanel != null) lbPanel.SetActive(true);
        if (lbPopup != null) lbPopup.SetActive(false);
    }

    private void ShowLBPopup()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (lbPanel != null) lbPanel.SetActive(false);
        if (lbPopup != null) lbPopup.SetActive(true);
    }

    public void OnLBPopupDone()
    {
        SettingsManager.globalSave.name = inputField.text;
        SettingsManager.globalLeaderboard.saves.Add(SettingsManager.globalSave);
        Debug.Log("leaderboard count " + SettingsManager.globalLeaderboard.saves.Count);
        SettingsManager.saveLeaderboard();
        EmptySave();
        SettingsManager.globalSave = null;
        Start();
        ShowLB();
    }


    private IEnumerator Continue()
    {
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);
        SettingsManager.loadLevel();
    }
    public void OnContinueClick()
    {
        StartCoroutine(Continue());
    }


    private IEnumerator NewGame()
    {
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);
        SettingsManager.globalSave = new SaveData(1, 0, 0);
        SettingsManager.saveSave();
        SettingsManager.loadLevel();
    }
    public void OnNewGameClick()
    {
        StartCoroutine(NewGame());
    }

    private void EmptySave()
    {
        SettingsManager.deleteSave();
    }

    public void OnSettingsClick()
    {
        ShowSettings();
    }

    public void OnLBClick()
    {
        ShowLB();
    }

    public void OnToMenuClick()
    {
        ShowMainMenu();
    }

    public void OnQuitClick()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }

    // read from global leaderboard and add entries to the ui
    public void FillLeaderboard()
    {
        foreach (Transform child in proLB.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in classicLB.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var list = SettingsManager.globalLeaderboard.saves.ToList();
        list.Sort((x, y) => {
            int c = x.deaths.CompareTo(y.deaths);
            if (c != 0) return c;
            return x.playtime.CompareTo(y.playtime);
        });

        int classic_offset = 0;
        for (int i = 0; i < list.Count; i++){
            SaveData sd = list[i];
            if(sd.deaths == 0)
            {
                LBEntry ui = Instantiate(proLBprefab, proLB.transform).GetComponent<LBEntry>();
                ui.Set(this, sd, i + 1);
                classic_offset = i;
            }
            else
            {
                LBEntry ui = Instantiate(classicLBprefab, classicLB.transform).GetComponent<LBEntry>();
                ui.Set(this, sd, i - classic_offset);
            }

        }
    }

    
}
