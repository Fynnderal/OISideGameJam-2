using KBCore.Refs;
using System.Collections;
using UnityEngine;


/// <summary>
/// Controls the behaviour of falling spikes
/// </summary>
public class RumblingSpikes : ValidatedMonoBehaviour
{
    private Vector2 startingPos;

    [SerializeField] private float speed;
    [SerializeField] private float minAmount;
    [SerializeField] private float maxAmount;
    [SerializeField, Range(0.01f, 100000)] private float timeBeforeFall;
    [SerializeField, Self] private SpriteRenderer spriteRenderer;
    [SerializeField, Anywhere] private Collider2D col;
    [SerializeField, Anywhere] private Collider2D triggerCol;
    [SerializeField, Anywhere] private ParticleSystem particles;
    [SerializeField, Self] private Rigidbody2D rb;
    [SerializeField, Self] private AudioSource audioSource; 


    private bool shaking = false;
    private float currentAmount;
    private float time = 0;
    public Collider2D[] Colliders => GetComponents<BoxCollider2D>();
    void Awake()
    {
        startingPos =  transform.position; 
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.None;
    }

    void FixedUpdate()
    {
        if (!shaking) return;

        currentAmount = Mathf.Lerp(minAmount, maxAmount, time / timeBeforeFall);

        // Shake effect
        float temp = Mathf.Sin(Time.time * speed) * currentAmount;
        rb.MovePosition(new Vector2 (startingPos.x + temp, startingPos.y + temp));
       

        time += Time.fixedDeltaTime; 

        if (time >= timeBeforeFall)
        {
            shaking = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.gravityScale = 2.5f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shaking = true;
            triggerCol.enabled = false;

        }  
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10000, EnemyType.TRAP, this.gameObject);
        }else if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            collision.gameObject.GetComponent<IEnemy>().TakeDamage(10000, Suit.NONE, this.gameObject); 
        }

        spriteRenderer.enabled = false;
        col.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        particles.Play();
        audioSource.Play();
        StartCoroutine(DestroyObject());

    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSecondsRealtime(8f);
        Destroy(gameObject);    
    }
}
