using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Controls the behavior of the generator that turns off lasers when destroyed.
/// </summary>
public class LasersSwitchPlatform : ValidatedMonoBehaviour, IEnemy
{
    [SerializeField] private Lasers[] lasers;
    [SerializeField] private int health;
    [SerializeField, Anywhere] private ParticleSystem particles;
    [SerializeField, Self] private SpriteRenderer spriteRenderer;
    [SerializeField, Self] private Collider2D col;
    [SerializeField, Self] private Animator animator;
    [SerializeField] private Light2D _light;
    [SerializeField, Self] private AudioSource audioSource;

    public bool TakeDamage (int damage, Suit suit, GameObject attacker)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy();
        }

        spriteRenderer.color = Color.white;

        animator.Play("Damaged", 0, 0);
        audioSource.Play(); 
        return true;
    }



    private void Destroy()
    {
        foreach (var laser in lasers)
        {
            laser.TurnOff();
        }

        particles.Play();
        spriteRenderer.enabled = false;
        col.enabled = false;

        if (_light != null)
            _light.enabled = false;


    }
}
