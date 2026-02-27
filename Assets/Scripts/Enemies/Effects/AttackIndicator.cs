using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    private Vector2 moveDir = Vector2.up / 4;
    private float duration;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float flashSpeed = 5f;

    private Vector2 startPos;
    private Vector2 endPos;
    private float elapsed;

    [SerializeField] private Color color1 = Color.red;
    [SerializeField] private Color color2 = Color.yellow;

    public void Init(float length = 0.5f, Vector2 movement = default)
    {
        sprite.enabled = true;
        duration = (length <= 0f) ? 0.5f : length;
        moveDir = (movement == default) ? Vector2.up : movement;

        startPos = transform.position;
        endPos = startPos + moveDir;

        elapsed = 0f;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        transform.position = Vector2.Lerp(startPos, endPos, Mathf.Clamp01(elapsed / duration));

        float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
        sprite.color = Color.Lerp(color1, color2, t);
    }

}

