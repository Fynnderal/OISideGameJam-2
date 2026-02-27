using UnityEngine;

/// <summary>
/// Triggers lava movement in the third level.
/// </summary>
public class Thirdleveltrigger : MonoBehaviour
{
    [SerializeField] private Lava vertical;
    [SerializeField] private Lava horizontal;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        vertical.Stopped = false;
        horizontal.Stopped = true;
    }
}
