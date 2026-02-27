using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls the behavior of limestone wall lights based on the player's color state.
/// </summary>
public class limestonelights : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Light2D light2D;
    [SerializeField, Anywhere] private PlayerController playerController;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        if (!playerController.StateContext.IsBlack)
            light2D.enabled = true;
        else
            light2D.enabled = false;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        light2D.enabled = false;
    }
}
