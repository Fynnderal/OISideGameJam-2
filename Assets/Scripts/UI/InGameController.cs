using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;
using System;
using TMPro;
using Unity.VisualScripting;

public class InGameController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject bg;
    [SerializeField] Image hb_fill;
    [SerializeField] TextMeshProUGUI dash_ind;
    [SerializeField] TextMeshProUGUI jump_ind;
    [SerializeField] Image suit_ind_red;
    [SerializeField] Image suit_ind_black;
    [SerializeField] Image glide_ind;
    [SerializeField] Image hook_ind;

    [SerializeField] Text timerText;

    [SerializeField] private PlayableDirector _deathCutScene;


    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (bg != null) bg.SetActive(true);
    }

    public void ShowSettings()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (bg != null) bg.SetActive(true);
    }

    public void HideAll()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (bg != null) bg.SetActive(false);
    }

    public void OnContinueClick()
    {
        ResumeGame();
    }

    public void OnSaveClick()
    {
        SettingsManager.saveSave();
    }

    public void OnSettingsClick()
    {
        ShowSettings();
    }


    private IEnumerator ToMenu()
    {
        _deathCutScene.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        ResumeGame();
        SceneManager.LoadScene("Menu");
    }

    public void OnToMenuClick()
    {
        StartCoroutine(ToMenu());
    }

    public void PauseGame()
    {
        ShowMenu();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        HideAll();
        Time.timeScale = 1f;
    }

    public void SetHealth(float hp_relative)
    {
        hb_fill.fillAmount = Mathf.Clamp01(hp_relative);
    }

    public void SetDashActive(bool active)
    {
        if (active)
        {
            dash_ind.enabled = true;
        }
        else
        {
            dash_ind.enabled = false;
        }
    }

    public void SetAbility(bool gliding)
    {
        if (gliding)
        {
            glide_ind.enabled = true;
            hook_ind.enabled = false;
        }
        else
        {
            glide_ind.enabled = false;
            hook_ind.enabled = true;
        }
    }
    public void SetJumpActive(bool active, int count = 0)
    {
        jump_ind.text = "Jump: " + count.ToString();

        if (active)
            jump_ind.enabled = true;
        else
            jump_ind.enabled = false;
    }

    public void SetSuit(bool red)
    {
        suit_ind_black.gameObject.SetActive(!red);
        suit_ind_red.gameObject.SetActive(red);
    }

    public void SetTimer(double time)
    {
        if (timerText == null)
        {
            Debug.LogWarning("inGameController has no reference to timer!");
            return;
        }
        TimeSpan ts = TimeSpan.FromSeconds(time);
        string times = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        timerText.text = times;
    }
}
