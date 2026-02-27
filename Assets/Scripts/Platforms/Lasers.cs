using KBCore.Refs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

/// <summary>
/// Controls the behavior of laser traps based on the player's color state.
/// </summary>
public class Lasers : ValidatedMonoBehaviour
{
    [SerializeField] bool isRed;
    [SerializeField, Anywhere] PlayerController player;

    [SerializeField, Self] Collider2D laser;
    [SerializeField, Self] Animator animator;
    [SerializeField, Anywhere] Light2D _light;

    [SerializeField] private bool deactivated = false;
    

    bool isCharacterBlack;

    public bool IsRed => isRed;


    // Update is called once per frame
    void Update()
    {
        if (deactivated)
            return;

        isCharacterBlack = player.StateContext.IsBlack;
        if (isRed)
        {
            if (isCharacterBlack)
                laser.enabled = true;
            else
                laser.enabled = false;
        }
        else
        {
            if (isCharacterBlack)
                laser.enabled = false;
            else
                laser.enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10000, EnemyType.TRAP, this.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;

            collision.gameObject.GetComponent<IEnemy>().TakeDamage(10000, Suit.NONE, this.gameObject);
        }
    }

    public void TurnOn()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("working"))
            return;
        
        animator.Play("turningon");
        _light.enabled = true;
        laser.enabled = true;
        deactivated = false;
    }

    public void TurnOff(bool animation = true)
    {
        if (animation)
        {
            animator.Play("turningoff");
        }
        else
        {
            animator.Play("idle");
        }
        laser.enabled = false;
        _light.enabled = false;
        deactivated = true;
    }

    private void OnEnable()
    {
        if (deactivated)
        {
            TurnOff(false);
        }
        else
        {
            TurnOn();
        }
    }
}
