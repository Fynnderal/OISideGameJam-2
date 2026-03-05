using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles all sound effects for enemy NPCs.
/// </summary>
public class EnemySounds : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Walking Sounds")]
    [SerializeField] private AudioClip[] walkingSounds;
    [SerializeField] private float walkingSoundInterval = 0.4f;

    [Header("Attack Sounds")]
    [SerializeField] private AudioClip[] attackSounds;

    [Header("Damage Sounds")]
    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private float damageSoundCooldown = 0.1f;
    [SerializeField][Range(0.8f, 1.2f)] private float damagePitchMin = 0.9f;
    [SerializeField][Range(0.8f, 1.2f)] private float damagePitchMax = 1.1f;

    [Header("Critical Hit Sounds")]
    [SerializeField] private AudioClip[] critDamageSounds;
    [SerializeField] private int critDamageThreshold = 50;
    [SerializeField][Range(0f, 1f)] private float critDamageVolume = 1f;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;

    [Header("Player Detection Sound")]
    [SerializeField] private AudioClip playerDetectedSound;

    [Header("Special Sound")]
    [SerializeField] private AudioClip specialSound;
    [SerializeField][Range(0f, 1f)] private float specialVolume = 0.8f;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float walkingVolume = 0.5f;
    [SerializeField][Range(0f, 2f)] private float attackVolume = 0.8f;
    [SerializeField][Range(0f, 1f)] private float damageVolume = 0.7f;
    [SerializeField][Range(0f, 5f)] private float deathVolume = 1f;
    [SerializeField][Range(0f, 1f)] private float detectionVolume = 0.6f;

    [Header("Visibility Settings")]
    [SerializeField] private bool onlyPlayWhenVisible = true;
    [SerializeField] private bool alwaysPlayImportantSounds = true; // Death, damage, detection

    // Audio sources - we use multiple for overlapping sounds
    private AudioSource mainSource;          // For general sounds (attack, detection, death)
    private AudioSource walkingSource;       // Dedicated source for walking (can be stopped independently)
    private AudioSource damageSource;        // Dedicated source for damage with pitch variation

    // Cycling indices
    private int currentWalkingSoundIndex = 0;
    private int currentAttackSoundIndex = 0;
    private int currentDamageSoundIndex = 0;
    private int currentCritSoundIndex = 0;

    // Timers
    private float nextWalkingSoundTime = 0f;
    private float lastDamageSoundTime = -Mathf.Infinity;

    // Cached visibility check
    private Renderer enemyRenderer;
    private Camera mainCamera;
    private bool isVisible = false;
    private float visibilityCheckInterval = 0.1f;
    private float nextVisibilityCheckTime = 0f;

    private void Awake()
    {
        SetupAudioSources();
        
        // Cache renderer for visibility check
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }
        
        mainCamera = Camera.main;
    }

    private void SetupAudioSources()
    {
        // Main audio source for general sounds
        mainSource = gameObject.AddComponent<AudioSource>();
        mainSource.playOnAwake = false;
        mainSource.spatialBlend = 1f; // 3D sound
        if (sfxMixerGroup != null) mainSource.outputAudioMixerGroup = sfxMixerGroup;

        // Dedicated walking source (allows independent control)
        walkingSource = gameObject.AddComponent<AudioSource>();
        walkingSource.playOnAwake = false;
        walkingSource.spatialBlend = 1f;
        if (sfxMixerGroup != null) walkingSource.outputAudioMixerGroup = sfxMixerGroup;

        // Dedicated damage source (allows pitch variation without affecting other sounds)
        damageSource = gameObject.AddComponent<AudioSource>();
        damageSource.playOnAwake = false;
        damageSource.spatialBlend = 1f;
        if (sfxMixerGroup != null) damageSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    /// <summary>
    /// Check if enemy is visible to the main camera.
    /// Uses caching to avoid checking every frame.
    /// </summary>
    private bool IsVisible()
    {
        if (!onlyPlayWhenVisible) return true;
        
        // Cache visibility check to avoid per-frame overhead
        if (Time.time >= nextVisibilityCheckTime)
        {
            nextVisibilityCheckTime = Time.time + visibilityCheckInterval;
            
            if (enemyRenderer != null)
            {
                isVisible = enemyRenderer.isVisible;
            }
            else if (mainCamera != null)
            {
                // Fallback: check if position is in camera viewport
                Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
                isVisible = viewportPos.x >= -0.1f && viewportPos.x <= 1.1f &&
                            viewportPos.y >= -0.1f && viewportPos.y <= 1.1f &&
                            viewportPos.z > 0;
            }
            else
            {
                isVisible = true; // Default to visible if no camera
            }
        }
        
        return isVisible;
    }

    #region Walking Sounds

    /// <summary>
    /// Call this from Update() or FixedUpdate() while the enemy is walking.
    /// Automatically handles cycling and timing.
    /// </summary>
    public void UpdateWalkingSound(bool isWalking, float speed = 1f)
    {
        if (!isWalking || walkingSounds == null || walkingSounds.Length == 0)
        {
            return;
        }

        // Skip walking sounds if not visible
        if (!IsVisible()) return;

        if (Time.time >= nextWalkingSoundTime)
        {
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
    /// Play an attack/shooting sound. Cycles through available variations.
    /// </summary>
    public void PlayAttackSound()
    {
        if (attackSounds == null || attackSounds.Length == 0) return;
        
        // Skip if not visible (unless important)
        if (!IsVisible() && !alwaysPlayImportantSounds) return;

        AudioClip clip = attackSounds[currentAttackSoundIndex];
        mainSource.PlayOneShot(clip, attackVolume);

        currentAttackSoundIndex = (currentAttackSoundIndex + 1) % attackSounds.Length;
    }

    #endregion

    #region Damage Sounds

    /// <summary>
    /// Play a damage sound with smart handling for rapid consecutive hits.
    /// Automatically plays critical hit sound if damage exceeds threshold.
    /// </summary>
    public void PlayDamageSound(int damageAmount = 0)
    {
        // Always play damage sounds (important feedback)
        if (!IsVisible() && !alwaysPlayImportantSounds) return;
        
        if (Time.time - lastDamageSoundTime < damageSoundCooldown)
        {
            return;
        }

        lastDamageSoundTime = Time.time;

        // Check if this is a critical hit
        if (damageAmount >= critDamageThreshold && critDamageSounds != null && critDamageSounds.Length > 0)
        {
            PlayCritDamageSound();
            return;
        }

        // Play normal damage sound
        if (damageSounds == null || damageSounds.Length == 0) return;

        // PITCH VARIATION:
        // Randomizing pitch slightly makes repeated damage sounds less repetitive
        // This is a common game audio technique for impacts/hits
        damageSource.pitch = Random.Range(damagePitchMin, damagePitchMax);

        AudioClip clip = damageSounds[currentDamageSoundIndex];
        damageSource.PlayOneShot(clip, damageVolume);

        currentDamageSoundIndex = (currentDamageSoundIndex + 1) % damageSounds.Length;
    }

    /// <summary>
    /// Play critical damage sound for high damage hits.
    /// </summary>
    private void PlayCritDamageSound()
    {
        damageSource.pitch = 1f;

        AudioClip clip = critDamageSounds[currentCritSoundIndex];
        damageSource.PlayOneShot(clip, critDamageVolume);

        currentCritSoundIndex = (currentCritSoundIndex + 1) % critDamageSounds.Length;
    }

    #endregion

    #region Death Sound

    /// <summary>
    /// Play the death sound. Stops other sounds and plays at full duration.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSound == null) return;

        // Stop other sounds so death sound is clear
        StopWalkingSound();
        damageSource.Stop();

        // Create a temporary GameObject with AudioSource that supports volume > 1
        GameObject tempAudioObject = new GameObject("DeathSound");
        tempAudioObject.transform.position = transform.position;
        
        AudioSource tempSource = tempAudioObject.AddComponent<AudioSource>();
        tempSource.clip = deathSound;
        tempSource.volume = Mathf.Min(deathVolume, 1f); // AudioSource.volume is clamped to 1
        tempSource.spatialBlend = 1f; // 3D sound
        if (sfxMixerGroup != null) tempSource.outputAudioMixerGroup = sfxMixerGroup;
        
        // Use PlayOneShot for volume boost beyond 1f
        tempSource.PlayOneShot(deathSound, deathVolume);
        
        // Destroy after clip finishes
        Object.Destroy(tempAudioObject, deathSound.length + 0.1f);
    }

    #endregion

    #region Player Detection Sound

    /// <summary>
    /// Play when player enters the enemy's vision/detection range.
    /// </summary>
    public void PlayPlayerDetectedSound()
    {
        if (playerDetectedSound == null) return;

        mainSource.PlayOneShot(playerDetectedSound, detectionVolume);
    }

    #endregion

    #region Special Sound

    /// <summary>
    /// Play the special sound (e.g., shield block, dash, charge-up).
    /// </summary>
    public void PlaySpecialSound()
    {
        if (specialSound == null) return;
        if (!IsVisible() && !alwaysPlayImportantSounds) return;

        mainSource.PlayOneShot(specialSound, specialVolume);
    }

    #endregion
}
