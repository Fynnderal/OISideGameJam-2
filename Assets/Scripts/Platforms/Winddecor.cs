

using KBCore.Refs;
using UnityEngine;


/// <summary>
/// Controls the movement of wind's sprite
/// </summary>
public class Winddecor : ValidatedMonoBehaviour
{
    [SerializeField, Self] private Rigidbody2D rb;
    [SerializeField] private Vector2 windVelocity;
    [SerializeField] private Collider2D windEndCollider;
    private Vector2 _spawnPosition;
    private void Start()
    {
        _spawnPosition = rb.position;
    }

    private void OnEnable()
    {
        rb.linearVelocity = windVelocity;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == windEndCollider)
        {
            rb.position = _spawnPosition;
        }
    }
}
