using UnityEngine;


public class    FSMShooting : FSMState
{
    private PlayerController pc;
    private GameObject bullet;

    // attack speed (shots per second)
    private float atkSpeed;
    private float nextShotTime;

    private int damage;

    private float bulletSpeed;

    private int rayMask;

    private bool is_wizard;
    private LineRenderer aimLineRenderer;

    bool attackIndicatorShown = false;

    private NPCRange myNPC;
    
    // Line growth tracking
    private float lineGrowthStartTime;
    
    // Visual effects
    private float baseWidth = 0.01f;
    private float maxWidth = 0.10f;

    private GameObject gun;
    private bool isWalkingRange = false;

    private string reloadingParam = "Reloading";

    // Store gun tip position for particle effects
    private Vector3 gunTipPosition;

    // Add this field at the top with other fields
    private float lastCannonAngle = 0f;
    private const float minRotationThreshold = 0.5f; // Minimum degrees to count as "rotating"

    // Track if player is visible
    private bool playerWasVisible = false;

    public FSMShooting(NPCRange npc, PlayerController pc) : base(npc)
    {
        myNPC = npc as NPCRange;
        if (myNPC == null) {
            Debug.LogError("FSMShooting can only be used with NPCRange.");
        }
        this.pc = pc;
        bullet = myNPC.bullet;
        atkSpeed = Mathf.Max(0f, myNPC.atkSpeed);
        bulletSpeed = myNPC.bulletSpeed;
        damage = myNPC.damage;
        is_wizard = myNPC.is_wizard();
    }

    public override void Enter()
    {
        // include everything except enemies
        rayMask = ~(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Trigger"));
        // Don't set nextShotTime here for regular enemies - wait until player is visible
        nextShotTime = float.PositiveInfinity;
        playerWasVisible = false;
        
        lineGrowthStartTime = Time.time;
        if (npc is NPCWalkingRange) {
            gun = ((NPCWalkingRange)npc).gun;
            gun.SetActive(true);
            isWalkingRange = true;
        } else if (npc is NPCStandingRange) {
            gun = ((NPCStandingRange)npc).gun;
            isWalkingRange = false;
        }
        else {
            gun = null;
        }
        
        // Set up LineRenderer for wizard aiming ray
        if (is_wizard) {
            // Wizards always see the player, so start cooldown immediately
            nextShotTime = atkSpeed > 0f ? Time.time + (1f / atkSpeed) : float.PositiveInfinity;
            SetupWizardAimLine();
        }
    }

    /// <summary>
    /// Creates and configures a LineRenderer for the wizard's aim indicator.
    /// The line visually shows where the wizard will shoot.
    /// </summary>
    private void SetupWizardAimLine()
    {
        aimLineRenderer = npc.GetComponent<LineRenderer>();
        if (aimLineRenderer == null) {
            aimLineRenderer = npc.gameObject.AddComponent<LineRenderer>();
        }

        // Configure the line renderer
        aimLineRenderer.positionCount = 2;
        aimLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        aimLineRenderer.enabled = true;

        // Get the wizard's sprite renderer to match its sorting layer and set order above it
        SpriteRenderer wizardSprite = npc.GetComponent<SpriteRenderer>();
        if (wizardSprite != null)
        {
            aimLineRenderer.sortingLayerName = "ForeGround";
            aimLineRenderer.sortingOrder = 10;
        }
        else
        {
            // Fallback if sprite renderer not found
            aimLineRenderer.sortingLayerName = "ForeGround";
            aimLineRenderer.sortingOrder = 10;
        }
    }

    private GameObject getAttackIndicator()
    {
        return myNPC.attackIndicatorPrefab;
    }

    public override void Update()
    {
        if (pc == null || bullet == null) return;

        var npcRb = npc.GetComponent<Collider2D>();
        var pcRb = pc.GetComponent<Collider2D>();
        if (npcRb == null || pcRb == null) return;


        Vector2 origin = npcRb.bounds.center;
        if (is_wizard) origin += Vector2.up * 0.1f; // Wizards shoot from higher up
        Vector2 playerCenter = pcRb.bounds.center;
        Vector2 toPlayer = playerCenter - origin;
        Vector2 dir = toPlayer.normalized;

        Vector3 spawnPosition = CalculateBulletSpawnPosition(dir);


        if (is_wizard)
        {
            ShowAttackIndicator(); // Wizards always show indicator (they always hit)
            HandleWizardShooting(origin, playerCenter, dir, spawnPosition);
        }
        else
        {
            HandleRegularShooting(origin, dir, toPlayer, spawnPosition);
        }
    }

    /// <summary>
    /// Determines where the bullet should spawn based on gun position and direction.
    /// Also handles sprite flipping and stores gun tip position for particle effects.
    /// </summary>
    private Vector3 CalculateBulletSpawnPosition(Vector2 dir)
    {
        Vector3 spawnPosition = myNPC.transform.position;
        gunTipPosition = spawnPosition; // Default to NPC position
        
        if (dir.x != 0)
        {
            UpdateSpriteFlipping(dir);
            
            if (npc is NPCWalkingRange or NPCStandingRange)
            {
                spawnPosition = CalculateGunSpawnPosition(dir);
            }
        }
        
        return spawnPosition;
    }

    private void UpdateSpriteFlipping(Vector2 dir)
    {
        if (npc is NPCStandingRange) return; // Standing range NPCs do not flip their sprites
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
        if (npc is NPCRange) ((NPCRange)npc).spriteRenderer.flipX = dir.x > 0;
    }

    /// <summary>
    /// Calculates bullet spawn position at the tip of the gun barrel.
    /// Handles gun rotation to aim at the player.
    /// </summary>
    private Vector3 CalculateGunSpawnPosition(Vector2 dir)
    {
        Vector3 spawnPosition;
        SpriteRenderer spriteRenderer = gun.GetComponent<SpriteRenderer>();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        if (isWalkingRange)
        {
            spawnPosition = CalculateWalkingRangeGunPosition(dir, spriteRenderer, angle);
        }
        else
        {
            spawnPosition = CalculateStandingRangeGunPosition(angle);
        }
        
        return spawnPosition;
    }

    /// <summary>
    /// Handles gun positioning for walking enemies - flips gun sprite based on direction,
    /// rotates gun to aim at player, and calculates barrel tip position.
    /// Returns NPC position if player is too close (prevents shooting through walls).
    /// </summary>
    private Vector3 CalculateWalkingRangeGunPosition(Vector2 dir, SpriteRenderer spriteRenderer, float angle)
    {
        spriteRenderer.flipX = dir.x > 0;
    
        // Mirror gun's local X position when facing opposite direction
        if (dir.x > 0 && gun.transform.localPosition.x < 0)
            gun.transform.localPosition = new Vector3(-gun.transform.localPosition.x, gun.transform.localPosition.y, gun.transform.localPosition.z);
        if (dir.x < 0 && gun.transform.localPosition.x > 0)
            gun.transform.localPosition = new Vector3(-gun.transform.localPosition.x, gun.transform.localPosition.y, gun.transform.localPosition.z);

        float pixelPerUnit = gun.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        float barrelLength = 29f / pixelPerUnit;
        Vector3 spawnPosition;

        // Rotate gun based on direction
        if (dir.x > 0) // Facing right
        {
            gun.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else // Facing left
        {
            gun.transform.rotation = Quaternion.Euler(0f, 0f, angle - 180);
        }
        
        // Use the normalized direction to calculate spawn position
        spawnPosition = gun.transform.position + (Vector3)(dir.normalized * barrelLength);

        // Always store the gun tip position for particle effects
        gunTipPosition = spawnPosition;

        // Check if player is too close - spawn bullet at NPC position instead
        float distanceToSpawn = Vector3.Distance(myNPC.transform.position, spawnPosition);
        float distanceToPlayer = Vector3.Distance(myNPC.transform.position, pc.transform.position);
    
        if (distanceToSpawn > distanceToPlayer)
        {
            return myNPC.transform.position;
        }

        return spawnPosition;
    }

    /// <summary>
    /// Handles cannon positioning for stationary turrets - rotates cannon to aim at player.
    /// Plays mechanical sound when cannon is rotating.
    /// Returns NPC position if player is too close.
    /// </summary>
    private Vector3 CalculateStandingRangeGunPosition(float angle)
    {
        // Standing range - cannon only rotates, doesn't flip
        float targetAngle = angle - 90;
        gun.transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        // Play turning sound if cannon is rotating
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(lastCannonAngle, targetAngle));
        if (angleDifference > minRotationThreshold)
        {
            // Cannon is turning - play walking/mechanical sound
            myNPC.GetEnemySounds()?.UpdateWalkingSound(true, 1f);
        }
        lastCannonAngle = targetAngle;

        // Calculate spawn position at the tip of the cannon based on its rotation
        float cannonLength = gun.GetComponent<SpriteRenderer>().sprite.bounds.size.y - 0.3f;
        Vector3 barrelTipOffset = gun.transform.up * cannonLength;
        gunTipPosition = gun.transform.position + barrelTipOffset;

        float distanceToSpawn = Vector3.Distance(myNPC.transform.position, gunTipPosition);
        float distanceToPlayer = Vector3.Distance(myNPC.transform.position, pc.transform.position);

        if (distanceToSpawn > distanceToPlayer) {
            return myNPC.transform.position;
        }
        return gunTipPosition;
    }

    /// <summary>
    /// Spawns attack indicator above NPC shortly before shooting (0.5s before).
    /// Only shows once per shot cycle.
    /// </summary>
    private void ShowAttackIndicator()
    {
        if (getAttackIndicator() != null && !attackIndicatorShown && nextShotTime - Time.time <= 0.5f)
        {
            GameObject indicator = GameObject.Instantiate(getAttackIndicator(), myNPC.transform.position + Vector3.up * 0.5f, myNPC.transform.rotation);
            AttackIndicator ai = indicator.GetComponent<AttackIndicator>();
            ai.Init(atkSpeed);
            attackIndicatorShown = true;
        }
    }

    /// <summary>
    /// Wizards always hit - no line-of-sight check needed.
    /// Updates the aim line visual that grows/fades as shot charges up.
    /// </summary>
    private void HandleWizardShooting(Vector2 origin, Vector2 playerCenter, Vector2 dir, Vector3 spawnPosition)
    {
        // Calculate line growth based on time until next shot
        float timeUntilShot = nextShotTime - Time.time;
        float shotInterval = (atkSpeed > 0f) ? (1f / atkSpeed) : 1f;
        float growthProgress = 1f - Mathf.Clamp01(timeUntilShot / shotInterval);

        // Update the aiming line
        UpdateWizardAimLine(origin, playerCenter, growthProgress);
        
        if (Time.time >= nextShotTime)
        {
            ExecuteShot(dir, spawnPosition);
        }
    }

    /// <summary>
    /// Updates the wizard's aim line visual - grows wider and more opaque as shot charges.
    /// Line connects wizard to player center.
    /// </summary>
    private void UpdateWizardAimLine(Vector2 origin, Vector2 playerCenter, float growthProgress)
    {
        // Update the visible aiming line ending at player center
        if (aimLineRenderer != null)
        {
            aimLineRenderer.SetPosition(0, origin);
            aimLineRenderer.SetPosition(1, playerCenter);
            
            // Fade from transparent to opaque as shot approaches
            float currentAlpha = growthProgress;
            
            // Darker red color 
            Color startColor = new Color(0.75f, 0f, 0f, currentAlpha);
            Color endColor = new Color(0.75f, 0f, 0f, currentAlpha * 0.3f);
            
            aimLineRenderer.startColor = startColor;
            aimLineRenderer.endColor = endColor;
            
            // Width grows with charge progress
            float currentWidth = Mathf.Lerp(baseWidth, maxWidth, growthProgress);
            aimLineRenderer.startWidth = currentWidth;
            aimLineRenderer.endWidth = currentWidth;
        }
    }

    /// <summary>
    /// Regular enemies need line-of-sight to player.
    /// Uses raycast to check visibility - only starts attack cooldown when player is seen.
    /// Resets cooldown if player breaks line of sight.
    /// </summary>
    private void HandleRegularShooting(Vector2 origin, Vector2 dir, Vector2 toPlayer, Vector3 spawnPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, Mathf.Infinity, rayMask);
        // Debug raycast (optional)
        // Debug.DrawRay(origin, dir * toPlayer.magnitude, Color.red);

        bool playerIsVisible = hit.collider != null && hit.collider.CompareTag("Player");
        
        // Only show indicator and shoot if player is visible
        if (playerIsVisible)
        {
            // Player just became visible - start the cooldown timer
            if (!playerWasVisible)
            {
                float interval = (atkSpeed > 0f) ? (1f / atkSpeed) : float.PositiveInfinity;
                nextShotTime = Time.time + interval;
                playerWasVisible = true;
            }
            
            // Show attack indicator only when player is visible
            ShowAttackIndicator();
            
            if (Time.time >= nextShotTime)
            {
                ExecuteShot(dir, spawnPosition);
            }
        }
        else
        {
            // Player not visible - reset states
            playerWasVisible = false;
            attackIndicatorShown = false;
            nextShotTime = float.PositiveInfinity; // Reset cooldown
        }
    }

    /// <summary>
    /// Fires the shot, triggers reload animation, and resets cooldown timer.
    /// </summary>
    private void ExecuteShot(Vector2 dir, Vector3 spawnPosition)
    {
        myNPC.animator.ResetTrigger(reloadingParam);
        Shoot(dir, spawnPosition);
        myNPC.animator.SetTrigger(reloadingParam);
        attackIndicatorShown = false;
        // cooldown: seconds per shot = 1 / atkSpeed (if atkSpeed > 0)
        float interval = (atkSpeed > 0f) ? (1f / atkSpeed) : float.PositiveInfinity;
        nextShotTime = Time.time + interval;
    }

    public override void FixedUpdate()
    {
    }
    
    /// <summary>
    /// Instantiates bullet, plays attack sound, and triggers particle.
    /// </summary>
    private void Shoot(Vector2 dir, Vector3 spawnPosition)
    {
        GameObject newBullet = GameObject.Instantiate(bullet, spawnPosition, Quaternion.identity);
        var b = newBullet.GetComponent<Bullet>();
        
        if (b != null)
        {
            b.setUp(dir, bulletSpeed, damage, npc.GetEnemyType());
        }
        
        // Play attack sound
        myNPC.GetEnemySounds()?.PlayAttackSound();
        
        // Handle shoot particle separately
        GameObject shootParticle = myNPC.shootParticle;
        
        if (shootParticle == null)
        {
            return;
        }
        
        // Move particle to gun tip
        shootParticle.transform.position = gunTipPosition;
        
        // Try to get ParticleSystem - check on the object itself first
        ParticleSystem ps = shootParticle.GetComponent<ParticleSystem>();
        
        // If not on the object, check children
        if (ps == null)
        {
            ps = shootParticle.GetComponentInChildren<ParticleSystem>();
        }
        
        if (ps != null)
        {
            ps.Play();
        }
    }

    public override void Exit()
    {
        // Disable LineRenderer when exiting shooting state
        if (aimLineRenderer != null) {
            aimLineRenderer.enabled = false;
        }
        if (isWalkingRange && gun != null) {
            gun.SetActive(false);
        }
    }
}
