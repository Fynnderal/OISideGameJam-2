using UnityEngine;

/// <summary>
/// Controls the behavior of lava that follows the player either horizontally or vertically.
/// </summary>
public class Lava : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lowLimit;
    [SerializeField] private bool horizontal;
    [SerializeField] private Transform player;
    [SerializeField] private bool stopped;
    [SerializeField] private bool followThePlayer;


    public bool Stopped { get => stopped; set => stopped = value; } 

    // Update is called once per frame
    void Update()
    {
        if (stopped) return; 
        Vector3 delta = player.position - transform.position;
        if (horizontal)
        {
            //if (followThePlayer)
            //{
            //    transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
            //}


            if (Mathf.Abs(delta.x) >= lowLimit)
            {
                transform.position = new Vector3(player.position.x + lowLimit, transform.position.y, transform.position.z);
            }

            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }
        else
        {
            //if (followThePlayer)
            //    transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);

            if (Mathf.Abs(delta.y) >= lowLimit)
            {
                transform.position = new Vector3(transform.position.x, player.position.y - lowLimit, transform.position.z);
            }
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
