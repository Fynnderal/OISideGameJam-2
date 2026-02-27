using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls the behavior of an anchor point for the grappling hook.
/// </summary>
public class Anchor : ValidatedMonoBehaviour
{
    [SerializeField] Light2D anchorLight;
    [SerializeField, Self] CircleCollider2D anchorCollider;

    public float Radius => anchorCollider.radius;   

    private void Awake()
    {
        anchorLight = transform.GetChild(0).GetComponent<Light2D>();
        anchorLight.enabled = false;
    }

    public void Activate()
    {
        anchorLight.enabled = true; 
    }

    public void Deactivate()
    {
        anchorLight.enabled = false;
    }
}
