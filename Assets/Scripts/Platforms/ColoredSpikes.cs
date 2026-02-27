using KBCore.Refs;
using UnityEngine;

/// <summary>
/// Controls the behavior of colored spikes based on the player's color state.
/// </summary>
public class ColoredSpikes : ValidatedMonoBehaviour
{
    [SerializeField] bool isRed;
    [SerializeField, Anywhere] PlayerController player;

    GameObject spikes;


    bool isCharacterBlack;
    private void Awake()
    {
        spikes = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        isCharacterBlack = player.StateContext.IsBlack;

        if (isRed)
        {
            if (isCharacterBlack)
                spikes.SetActive(true);
            else
                spikes.SetActive(false);
        }
        else
        {
            if (isCharacterBlack)
                spikes.SetActive(false);
            else
                spikes.SetActive(true);
        }
    }
    
}
