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

    [SerializeField] private Button continueButton;



    [SerializeField] private PlayableDirector _deathCutScene;

    void Start()
    {
        continueButton.interactable = PlayerPrefs.GetInt("save") == 1;
        ShowMainMenu();
        Cursor.visible = true;
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

    }

    private IEnumerator Continue()
    {
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Continue");
    }
    public void OnContinueClick()
    {
        StartCoroutine(Continue());
    }


    private IEnumerator NewGame()
    {
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.SetInt("save", 1);
        SceneManager.LoadScene("NewGame");
    }
    public void OnNewGameClick()
    {
        StartCoroutine(NewGame());
    }


    public void OnSettingsClick()
    {
        ShowSettings();
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



    
}
