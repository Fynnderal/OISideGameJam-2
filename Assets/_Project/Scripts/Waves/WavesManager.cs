using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WavesManager : MonoBehaviour
{
    [SerializeField] GameObject[] _waves;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] UpgradeManager _upgradeManager;
    [SerializeField] int _maxNumberOfWaves;
    [SerializeField] TrapManager _trapManager;
    [SerializeField] TextMeshProUGUI _waveText;


    int _waveID = 0;
    GameObject _currentWave;

    private void OnEnable()
    {
        _waveID = 0;
        SpawnWave();
    }

    void Update()
    {
        if (_currentWave == null) return;

        if (_currentWave.transform.childCount == 0)
        {
            NextWave();
        }

        _waveText.text = $"Wave: {_waveID}/{_maxNumberOfWaves}";
    }
    void NextWave()
    {
        Destroy(_currentWave);
        _currentWave = null;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        if (_waveID > _maxNumberOfWaves)
        {
            EndArena();
            return;
        }


        _upgradeManager.ShowUpgrades();


    }
    public void SpawnWave()
    {
        if (_waveID < _waves.Length)
            _currentWave = Instantiate(_waves[_waveID], _spawnPoint.position, Quaternion.identity);
        else {
            _currentWave = new GameObject();
            _trapManager.InstantiateEnemies(_waveID + 1, _currentWave.transform);
            
        }
        _waveID++;
    }

    void EndArena()
    {
        SceneManager.LoadScene("Menu");
    }

}
