using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Controls breakable platforms that the player can break by dashing through them.
/// <summary>
public class BreakablePlatforms : ValidatedMonoBehaviour
{
    [SerializeField] private bool horizontal;
    [SerializeField, Anywhere] private PlayerController player;
    [SerializeField] private ParticleSystem particles;
    [SerializeField, Self] private Collider2D col;
    [SerializeField, Self] private SpriteRenderer spriteRenderer;
    [SerializeField, Self] private AudioSource audioSource;


    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if (collision.collider.CompareTag("Player"))
        {
            if (horizontal && player.StateContext.DownDashTimer.isRunning)
            {
                Break();
            }
            else if (!horizontal && player.StateContext.SideDashTimer.isRunning)
            {
                Break();
            }
        }
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
    private void Break()
    {
        particles.Play();
        audioSource.Play();
        spriteRenderer.enabled = false;
        col.enabled = false;

    }
}
