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
    [SerializeField] private GameObject bg;

    [SerializeField] private GameObject deathMenu;
    [SerializeField] private PlayerControllerTopDown _playerController; 

    [SerializeField] private PlayableDirector _deathCutScene;

    [SerializeField] private GameObject NewGameMenu;

    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (bg != null) bg.SetActive(true);
    }


    public void HideAll()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (bg != null) bg.SetActive(false);
    }

    public void OnContinueClick()
    {
        _playerController.Pause(false);
        ResumeGame();
    }

    public void OnLetsGoClick()
    {
        _playerController.ExitGreetings();
        NewGameMenu.SetActive(false);
    }

    public void ShowDeathMenu()
    {
        if (deathMenu != null) deathMenu.SetActive(true);
    }
    private IEnumerator Restart()
    {
        _deathCutScene.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("Continue");
    }

    public void OnRestartClick()
    {
        StartCoroutine(Restart());
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

    
}
