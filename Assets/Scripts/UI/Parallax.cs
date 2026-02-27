using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] bool roundDown = false;
    public float strength = 0.5f;
    private float spriteWidth;

    private Transform cam;
    private Vector3 lastCamPos;

    // setup parallax around the camera
    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
        spriteWidth = GetComponentInChildren<SpriteRenderer>().bounds.size.x;

        if (roundDown) 
            spriteWidth = Mathf.FloorToInt(spriteWidth);
    }

    // update positions of the objects
    void Update()
    {
        Vector3 delta = -cam.position + lastCamPos;
        delta.x *= (1 - strength);
        delta.y = 0;
        transform.position += delta;

        foreach (Transform child in transform)
        {
            if (cam.position.x - child.position.x > spriteWidth)
                child.position += Vector3.right * spriteWidth * 2f;
            else if (cam.position.x - child.position.x < -spriteWidth)
                child.position -= Vector3.right * spriteWidth * 2f;

        }

        lastCamPos = cam.position;
    }
}
