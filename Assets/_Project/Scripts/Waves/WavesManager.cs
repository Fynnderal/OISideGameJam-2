using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] GameObject[] _waves;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] UpgradeManager _upgradeManager;


    int _waveID = 0;
    GameObject _currentWave;

    private void OnEnable()
    {
        _waveID = 0;
        SpawnWave();
    }

    void Update()
    {
        if (_currentWave.transform.childCount == 0)
        {
            NextWave();
        }
    }
    void NextWave()
    {
        Destroy(_currentWave);

        if (_waveID == _waves.Length)
        {
            EndArena();
            return;
        }


        _upgradeManager.ShowUpgrades();


    }
    public void SpawnWave()
    {
        _currentWave = Instantiate(_waves[_waveID], _spawnPoint.position, Quaternion.identity);
        _waveID++;
    }

    void EndArena()
    {
        Debug.Log("You Won!");
    }

}
