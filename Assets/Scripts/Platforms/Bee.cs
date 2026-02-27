using KBCore.Refs;
using UnityEngine;

public class Bee : ValidatedMonoBehaviour, IEnemy
{
    [SerializeField] ParticleSystem _particles;
    [SerializeField, Self] SpriteRenderer _spriteRenderer;
    [SerializeField, Self] Collider2D _collider2D;
    public bool TakeDamage(int damdage, Suit suit, GameObject player)
    {
        _particles.Play();
        _spriteRenderer.enabled = false;
        _collider2D.enabled = false;

        return true;
    }
}
