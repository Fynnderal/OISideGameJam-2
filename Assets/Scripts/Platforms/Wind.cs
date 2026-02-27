using KBCore.Refs;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls wind behavior that affects the player when gliding.
/// </summary>
public class Wind : ValidatedMonoBehaviour
{

    [SerializeField] private Vector2 windForce;
    [SerializeField, Anywhere] private PlayerController player;
    [SerializeField] private Vector2 maxSpeed;
    bool _isPlayerInside = false;
    Rigidbody2D _playerRB;
    private void Start()
    {
        _playerRB = player.GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (maxSpeed.x > 0 && _playerRB.linearVelocityX > maxSpeed.x)
            return;

        if (maxSpeed.x < 0 && _playerRB.linearVelocityX < maxSpeed.x)
            return;

        if (maxSpeed.y > 0 && _playerRB.linearVelocityY > maxSpeed.y)
            return;

        if (maxSpeed.y < 0 && _playerRB.linearVelocityY < maxSpeed.y)
            return;

        if (_isPlayerInside)
        {
            _playerRB.AddForce(windForce);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInside = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (collision.gameObject.GetComponent<PlayerController>().StateContext.IsGliding)
            {
                _isPlayerInside = true;
            }
            else
            {
                _isPlayerInside = false;
            }
        }
    }
}
