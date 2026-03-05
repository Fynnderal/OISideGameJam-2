using KBCore.Refs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Upgrade
{
    public string title;
    public Sprite icon;
    public Action effect;
}
public class UpgradeManager : ValidatedMonoBehaviour
{
    private Upgrade[] _upgrades;

    [Header("Data")]
    [SerializeField] private Sprite[] _cardIcons;
    [SerializeField] private int _cardNumber;
    [SerializeField] private string[] _title;

    [Header("UI")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI[] _titleFields;
    [SerializeField] private Button[] _cardButtons;
    [SerializeField] private Image[] _cardIconFields;


    [SerializeField] private WavesManager _waveManager;
    [SerializeField] private TrapManager _trapManager;


    [SerializeField] private PlayerControllerTopDown _player;
    private void Awake()
    {
        Debug.Log($"UpgradeManager instance: {gameObject.name}", this);
        _upgrades = new Upgrade[_cardNumber];  
        
        for (int i = 0; i < _cardNumber; i++)
        {
            _upgrades[i] = new Upgrade()
            {
                title = _title[i],
                icon = _cardIcons[i], 
            };
        }

        _upgrades[0].effect = () =>
        {
            _trapManager.InstantiateTraps(TrapType.Fire);
            Debug.Log("Instantiated fire trap");
        };
        _upgrades[1].effect = () =>
        {
            _trapManager.InstantiateTraps(TrapType.Spike);
        };
        _upgrades[2].effect = () =>
        {
            if (_player._pistolDamageUpgrade < 2f)
            {
                _player._pistolDamageUpgrade += 0.1f;
            }
        };
        _upgrades[3].effect = () =>
        {
            if (_player._smgDamageUpgrade < 2f)
            {
                _player._smgDamageUpgrade += 0.1f;
            }
        };
        _upgrades[4].effect = () =>
        {
            if (Spikes.damageUpgrade < 2f)
            {
                Spikes.damageUpgrade += 0.1f;
            }
        };

        _upgrades[5].effect = () =>
        {
            if (FireTrap.damageUpgrade < 2f)
            {
                FireTrap.damageUpgrade += 0.1f;
            }
        };
        _upgrades[6].effect = () =>
        {
            if (Fire.durationUpgrade < 2f)
            {
                Fire.durationUpgrade += 0.1f;
            }
        };
        _upgrades[7].effect = () =>
        {
            if (Fire.damageUpgrade < 2f)
            {
                Fire.damageUpgrade += 0.1f;
            }
        };
    }


    private void Start()
    {
    }

    public void ShowUpgrades()
    {
        int[] upgrades = new int[3];

        for (int i = 0; i < 3; i++)
        {
            upgrades[i] = UnityEngine.Random.Range(0, _upgrades.Length);
        }

        for (int j = 0; j < 3; j++)
        {
            int i = j;
            int m = upgrades[i];
            _titleFields[i].text = _upgrades[upgrades[i]].title;
            _cardIconFields[i].sprite = _upgrades[upgrades[i]].icon;
            Debug.Log($"Assigned upgrade index: {m} to {i}");
            _cardButtons[i].onClick.RemoveAllListeners();
            _cardButtons[i].onClick.AddListener(() => {
                _upgrades[m].effect.Invoke();

                Debug.Log($"Clicked upgrade index: {m}");
                Debug.Log(_upgrades[m].effect == null
                    ? "EFFECT IS NULL"
                    : "EFFECT EXISTS");

                _panel.SetActive(false);
                Cursor.visible = false;
                _waveManager.SpawnWave();
            });

        }

        _panel.SetActive(true);
        Cursor.visible = true;
    }
    

}
