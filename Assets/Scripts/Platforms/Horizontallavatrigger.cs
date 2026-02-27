using Unity.VisualScripting;
using UnityEngine;


//// <summary>
/// This trigger activates horizontal lava and deactivates vertical lava at the 3. level
///  </summary>
public class Horizontallavatrigger : MonoBehaviour
{
    [SerializeField] Lava vertical;
    [SerializeField] Lava horizontal;
    [SerializeField] RumblingSpikes[] rumblingSpikes;
    [SerializeField] Collider2D rumbleTrigger;
    [SerializeField] Collider2D _collider;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        horizontal.Stopped = false;

        
        _collider.enabled = false;
        vertical.Stopped = true;
        foreach (var spike in rumblingSpikes)
        {
            foreach(var col in spike.Colliders)
                col.enabled = true;
        }
        rumbleTrigger.enabled = true;
    }
    void Update()
    {

    }
}
