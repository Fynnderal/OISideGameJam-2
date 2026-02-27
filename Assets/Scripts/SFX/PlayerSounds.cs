using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles all sound effects for the player.
/// </summary>
public class PlayerSounds : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Walking Sounds")]
    [SerializeField] private AudioClip[] walkingSounds;
    [SerializeField] private float walkingSoundInterval = 0.3f;

    [Header("Attack Sounds")]
    [SerializeField] private AudioClip[] attackSounds;

    [Header("Damage Sounds")]
    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private float damageSoundCooldown = 0.1f;
    [SerializeField][Range(0.8f, 1.2f)] private float damagePitchMin = 0.9f;
    [SerializeField][Range(0.8f, 1.2f)] private float damagePitchMax = 1.1f;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;

    [Header("Jump Sound")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Dash Sound")]
    [SerializeField] private AudioClip dashSound;

    [Header("Land Sound")]
    [SerializeField] private AudioClip landSound;

    [Header("Suit Change")]
    [SerializeField] private AudioClip suitChangeSound;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float walkingVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float attackVolume = 0.8f;
    [SerializeField][Range(0f, 1f)] private float damageVolume = 0.7f;
    [SerializeField][Range(0f, 1f)] private float deathVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float jumpVolume = 0.6f;
    [SerializeField][Range(0f, 1f)] private float dashVolume = 0.7f;
    [SerializeField][Range(0f, 1f)] private float landVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float suitChangeVolume = 0.8f;

    // Audio sources
    private AudioSource mainSource;
    private AudioSource walkingSource;
    private AudioSource damageSource;

    // Cycling indices
    private int currentWalkingSoundIndex = 0;
    private int currentAttackSoundIndex = 0;
    private int currentDamageSoundIndex = 0;

    // Timers
    private float nextWalkingSoundTime = 0f;
    private float lastDamageSoundTime = -Mathf.Infinity;

    private void Awake()
    {
        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        // Main audio source for general sounds
        mainSource = gameObject.AddComponent<AudioSource>();
        mainSource.playOnAwake = false;
        mainSource.spatialBlend = 0f; // 2D sound for player
        if (sfxMixerGroup != null) mainSource.outputAudioMixerGroup = sfxMixerGroup;

        // Dedicated walking source
        walkingSource = gameObject.AddComponent<AudioSource>();
        walkingSource.playOnAwake = false;
        walkingSource.spatialBlend = 0f;
        if (sfxMixerGroup != null) walkingSource.outputAudioMixerGroup = sfxMixerGroup;

        // Dedicated damage source (allows pitch variation)
        damageSource = gameObject.AddComponent<AudioSource>();
        damageSource.playOnAwake = false;
        damageSource.spatialBlend = 0f;
        if (sfxMixerGroup != null) damageSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    #region Walking Sounds

    /// <summary>
    /// Call this from Update() while the player is walking.
    /// </summary>
    public void UpdateWalkingSound(bool isWalking, float speed = 1f)
    {
        if (!isWalking || walkingSounds == null || walkingSounds.Length == 0) {
            return;
        }

        if (Time.time >= nextWalkingSoundTime) {
            PlayWalkingSound();
            nextWalkingSoundTime = Time.time + (walkingSoundInterval / speed);
        }
    }

    private void PlayWalkingSound()
    {
        if (walkingSounds.Length == 0) return;

        AudioClip clip = walkingSounds[currentWalkingSoundIndex];
        walkingSource.PlayOneShot(clip, walkingVolume);

        currentWalkingSoundIndex = (currentWalkingSoundIndex + 1) % walkingSounds.Length;
    }

    /// <summary>
    /// Stops any currently playing walking sound.
    /// </summary>
    public void StopWalkingSound()
    {
        walkingSource.Stop();
    }

    #endregion

    #region Attack Sounds

    /// <summary>
    /// Play an attack sound. Cycles through available variations.
    /// </summary>
    public void PlayAttackSound()
    {
        if (attackSounds == null || attackSounds.Length == 0) return;

        AudioClip clip = attackSounds[currentAttackSoundIndex];
        mainSource.PlayOneShot(clip, attackVolume);

        currentAttackSoundIndex = (currentAttackSoundIndex + 1) % attackSounds.Length;
    }

    #endregion

    #region Damage Sounds

    /// <summary>
    /// Play a damage sound with pitch variation.
    /// </summary>
    public void PlayDamageSound()
    {
        if (damageSounds == null || damageSounds.Length == 0) return;

        if (Time.time - lastDamageSoundTime < damageSoundCooldown) {
            return;
        }

        lastDamageSoundTime = Time.time;

        damageSource.pitch = Random.Range(damagePitchMin, damagePitchMax);

        AudioClip clip = damageSounds[currentDamageSoundIndex];
        damageSource.PlayOneShot(clip, damageVolume);

        currentDamageSoundIndex = (currentDamageSoundIndex + 1) % damageSounds.Length;
    }

    #endregion

    #region Death Sound

    /// <summary>
    /// Play the death sound.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSound == null) return;

        StopWalkingSound();
        damageSource.Stop();

        mainSource.pitch = 1f;
        mainSource.PlayOneShot(deathSound, deathVolume);
    }

    #endregion

    #region Jump Sound

    /// <summary>
    /// Play the jump sound.
    /// </summary>
    public void PlayJumpSound()
    {
        if (jumpSound == null) return;

        mainSource.PlayOneShot(jumpSound, jumpVolume);
    }

    #endregion

    #region Dash Sound

    /// <summary>
    /// Play the dash sound.
    /// </summary>
    public void PlayDashSound()
    {
        if (dashSound == null) return;

        mainSource.PlayOneShot(dashSound, dashVolume);
    }

    #endregion

    #region Land Sound

    /// <summary>
    /// Play the land sound (when player lands on ground).
    /// </summary>
    public void PlayLandSound()
    {
        if (landSound == null) return;

        mainSource.PlayOneShot(landSound, landVolume);
    }

    #endregion

    #region Suit Change Sound
    /// <summary>
    /// Play the suit change sound.
    /// </summary>
    public void PlaySuitChangeSound()
    {
        if (suitChangeSound == null) return;

        mainSource.PlayOneShot(suitChangeSound, suitChangeVolume);
    }
    #endregion
}
