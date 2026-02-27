using KBCore.Refs;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


/// <summary>
/// Loads the next level when the player enters the trigger.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] LevelSetupObject _levelSetupObject;
    [SerializeField] bool _toMainMenu = false;
    [SerializeField] PlayableDirector  _deathCutScene;
    [SerializeField] PlayableDirector  _respawnCutScene;
    [SerializeField] PlayerController _player;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadLevel());
        }
    }

    private IEnumerator LoadLevel()
    {
        _deathCutScene.Play();
        yield return new WaitForSeconds(0.5f);

        if (_toMainMenu)
        {
            SettingsManager.FinishGame();
            Cursor.visible = true;
            yield return null;
        }

        _levelSetupObject.LoadNextLevel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_player != null)
            _player.Input.DisablePlayerActions();
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.1f);
        _respawnCutScene.Play();
        yield return new WaitForSeconds(0.5f);
        if (_player != null)
            _player.Input.EnablePlayerActions();
    }
}
