using UnityEngine;
using UnityEngine.Audio;

public class MenuMusic : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioClip menuTrack;
    [SerializeField][Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool loop = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = menuTrack;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound for music

        if (musicMixerGroup != null) {
            audioSource.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    private void Start()
    {
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
