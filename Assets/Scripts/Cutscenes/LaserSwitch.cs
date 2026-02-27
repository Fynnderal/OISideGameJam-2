using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [SerializeField] Lasers laser;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        laser.TurnOff();
    }
}
