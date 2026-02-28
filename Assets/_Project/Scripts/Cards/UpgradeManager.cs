using KBCore.Refs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Upgrade
{
    public string title;
    public string description;
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
    [SerializeField] private string[] _description;

    [Header("UI")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI[] _titleFields;
    [SerializeField] private TextMeshProUGUI[] _descriptionFields;
    [SerializeField] private Button[] _cardButtons;
    [SerializeField] private Image[] _cardIconFields;

    private void Awake()
    {
        _upgrades = new Upgrade[_cardNumber];  
        
        for (int i = 0; i < _cardNumber; i++)
        {
            _upgrades[i] = new Upgrade()
            {
                title = _title[i],
                description = _description[i],
                icon = _cardIcons[i], 
                effect = () => { Debug.Log("Upgrade " + i); }
            };
        }
    }


    private void Start()
    {
        ShowUpgrades();
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
            _titleFields[i].text = _upgrades[upgrades[i]].title;
            _descriptionFields[i].text = _upgrades[upgrades[i]].description;
            _cardIconFields[i].sprite = _upgrades[upgrades[i]].icon;
            _cardButtons[i].onClick.RemoveAllListeners();
            _cardButtons[i].onClick.AddListener(() => {
            _upgrades[upgrades[i]].effect.Invoke(); 
            _panel.SetActive(false);
            });

        }

        _panel.SetActive(true); 
    }
    

}
