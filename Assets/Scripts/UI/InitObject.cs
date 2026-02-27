using UnityEngine;
using UnityEngine.Audio;

// opbject for initializing whole project settings
public class InitObject : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    public void Awake()
    {
        AudioManager.mixer = mixer;
        Debug.Log("AudioManager mixer set");

        Destroy(gameObject);
    }
}

