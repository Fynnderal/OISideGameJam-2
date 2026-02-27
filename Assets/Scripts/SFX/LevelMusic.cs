using UnityEngine;
using UnityEngine.Audio;

public class LevelMusic : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField][Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool loop = true;
    [SerializeField] public AudioClip CalmMusic;
    [SerializeField] public AudioClip CombatMusic;

    [Header("Crossfade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Arenas")]
    [SerializeField] private ArenaController[] Arenas;

    private AudioSource calmSource;
    private AudioSource combatSource;
    private bool isInCombat = false;

    private void Awake()
    {
        // Create two audio sources for crossfading
        calmSource = gameObject.AddComponent<AudioSource>();
        combatSource = gameObject.AddComponent<AudioSource>();

        SetupAudioSource(calmSource, CalmMusic);
        SetupAudioSource(combatSource, CombatMusic);
    }

    private void SetupAudioSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = loop;
        source.volume = 0f;
        if (musicMixerGroup != null)
        {
            source.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    void Start()
    {
        // Calm starts at full volume, combat at zero
        calmSource.volume = volume;
        combatSource.volume = 0f;
        // Start both tracks simultaneously
        calmSource.Play();
        combatSource.Play();
    }

    void Update()
    {
        bool shouldBeInCombat = IsInCombat();

        if (shouldBeInCombat != isInCombat)
        {
            isInCombat = shouldBeInCombat;
        }

        // Gradually fade volumes
        if (isInCombat)
        {
            // Fade in combat, fade out calm
            combatSource.volume = Mathf.MoveTowards(combatSource.volume, volume, (volume / fadeDuration) * Time.deltaTime);
            calmSource.volume = Mathf.MoveTowards(calmSource.volume, 0f, (volume / fadeDuration) * Time.deltaTime);
        }
        else
        {
            // Fade in calm, fade out combat
            calmSource.volume = Mathf.MoveTowards(calmSource.volume, volume, (volume / fadeDuration) * Time.deltaTime);
            combatSource.volume = Mathf.MoveTowards(combatSource.volume, 0f, (volume / fadeDuration) * Time.deltaTime);
        }
    }

    private bool IsInCombat()
    {
        foreach (var arena in Arenas)
        {
            if (arena.IsArenaActive())
            {
                return true;
            }
        }
        return false;
    }
}
