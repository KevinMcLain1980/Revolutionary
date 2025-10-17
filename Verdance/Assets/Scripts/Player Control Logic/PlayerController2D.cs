using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Main player controller handling movement, jumping, combat, magic, and damage
public class PlayerController2D : MonoBehaviour
{
    // Movement configuration
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f; // Base horizontal movement speed
    private Vector2 moveInput; // Current input direction
    private float currentSpeedMultiplier = 1f; // Speed modifier for buffs/debuffs

    // Jumping configuration
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f; // Upward force applied when jumping
    [SerializeField] private Transform groundCheck; // Position to check if player is grounded
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius of ground detection circle
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground detection

    // Attack configuration
    [Header("Attack")]
    [SerializeField] private GameObject thornbrandHitbox; // Weapon hitbox for legacy attacks
    [SerializeField] private float attackCooldown = 0.5f; // Time between legacy attacks
    [SerializeField] private Animator animator; // Animator for player animations
    private bool canAttack = true; // Whether player can perform legacy attack

    // Magic spell configuration
    [Header("Magic")]
    [SerializeField] private MagicManager magicManager; // Manager for casting spells
    [SerializeField] private float windStepCooldown = 5f; // Cooldown for WindStep spell
    [SerializeField] private float lightPulseCooldown = 8f; // Cooldown for LightPulse spell
    private bool canCastWindStep = true; // Whether WindStep is off cooldown
    private bool canCastLightPulse = true; // Whether LightPulse is off cooldown

    // Damage visual feedback configuration
    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Sprite renderer for color flashing
    [SerializeField] private Color flashColor = Color.red; // Color to flash when damaged
    [SerializeField] private float flashDuration = 0.1f; // Duration of each flash
    [SerializeField] private int flashCount = 3; // Number of flashes when damaged
    [SerializeField] private float knockbackDuration = 0.3f; // Duration of knockback state

    // Audio configuration
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for playing sounds
    [SerializeField] private AudioClip hurtSound; // Sound played when damaged
    [SerializeField] private AudioClip deathSound; // Sound played on death
    [SerializeField] private AudioClip jumpSound; // Sound played when jumping
    [SerializeField] private AudioClip runningSound; // Looping sound for running
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f; // Volume for sound effects
    [Range(0f, 1f)][SerializeField] private float runningVolume = 0.5f; // Volume for running sound
    private bool isPlayingRunSound = false; // Whether running sound is currently playing

    // Component and state references
    private Rigidbody2D rb; // Rigidbody for physics movement
    private CapsuleCollider2D cc; // Collider for collision detection
    private Color originalColor; // Original sprite color for damage flash
    private bool isKnockedBack = false; // Whether player is in knockback state
    private bool isInvincible = false; // Whether player is invincible (i-frames)
    private bool isDead = false; // Whether player is dead
    private int currentHealth; // Current health points
    [SerializeField] private int maxHealth = 5; // Maximum health points

    // Initialize components on awake
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Initialize health on start
    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Main update loop for animations and audio
    private void Update()
    {
        UpdateAnimationStates();
        HandleRunningSound();
    }

    // Physics updates in FixedUpdate to prevent sticking issues
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Enable or disable collision with enemies (used during knockback)
    public void SetPhaseThrough(bool value)
    {
        if (cc != null)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null)
                {
                    Physics2D.IgnoreCollision(cc, enemyCollider, value);
                }
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsPhasing", value);
        }
    }

    // Apply horizontal movement based on input
    private void MovePlayer()
    {
        if (isKnockedBack) return;
        Vector2 newVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }

    // Update animator and flip sprite based on movement
    private void UpdateAnimationStates()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        if (moveInput.x > 0.1f) spriteRenderer.flipX = false;
        else if (moveInput.x < -0.1f) spriteRenderer.flipX = true;
    }

    // Play or stop running sound based on movement state
    private void HandleRunningSound()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f && IsGrounded() && !isKnockedBack && !isDead;

        if (isMoving && !isPlayingRunSound && runningSound != null && audioSource != null)
        {
            audioSource.clip = runningSound;
            audioSource.loop = true;
            audioSource.volume = runningVolume;
            audioSource.Play();
            isPlayingRunSound = true;
        }
        else if (!isMoving && isPlayingRunSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
            isPlayingRunSound = false;
        }
    }

    // Handle movement input from new input system
    public void OnMove(InputValue value)
    {
        if (isKnockedBack) return;
        moveInput = value.Get<Vector2>();
    }

    // Handle jump input and play jump sound
    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("IsJumping");

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound, sfxVolume);
            }
        }
    }

    // Check if player is on the ground
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // Handle attack input, using PlayerCombat if available
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.PerformAttack();
            }
            else
            {
                TryAttack();
            }
        }
    }

    // Legacy attack method using thornbrand hitbox
    private void TryAttack()
    {
        if (!canAttack) return;
        animator.SetTrigger("AttackTrigger");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    // Reset attack cooldown
    private void ResetAttack()
    {
        canAttack = true;
    }

    // Activate thornbrand hitbox for legacy attack (called by animation event)
    public void ActivateThornbrandHitbox()
    {
        if (thornbrandHitbox != null)
        {
            // Position hitbox based on player facing direction
            float direction = spriteRenderer.flipX ? -1 : 1;
            Vector3 localPos = thornbrandHitbox.transform.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * direction;
            thornbrandHitbox.transform.localPosition = localPos;

            thornbrandHitbox.SetActive(true);
            StartCoroutine(DisableHitboxAfterDelay(0.2f));
        }
    }

    // Disable hitbox after specified delay
    private IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        thornbrandHitbox.SetActive(false);
    }

    // Use item from inventory
    public void UseInventoryItem(int slotIndex)
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.UseItem(slotIndex);
        }
    }

    // Handle WindStep spell cast input
    public void OnCastWindStep(InputValue value)
    {
        if (value.isPressed && canCastWindStep)
        {
            magicManager?.CastSpell("WindStep");
            canCastWindStep = false;
            Invoke(nameof(ResetWindStep), windStepCooldown);
        }
    }

    // Handle LightPulse spell cast input
    public void OnCastLightPulse(InputValue value)
    {
        if (value.isPressed && canCastLightPulse)
        {
            magicManager?.CastSpell("LightPulse");
            canCastLightPulse = false;
            Invoke(nameof(ResetLightPulse), lightPulseCooldown);
        }
    }

    // Reset spell cooldowns
    private void ResetWindStep() => canCastWindStep = true;
    private void ResetLightPulse() => canCastLightPulse = true;

    // Modify player speed permanently
    public void ModifySpeed(float multiplier) => currentSpeedMultiplier = multiplier;

    // Modify player speed for a duration
    public void ModifySpeed(float multiplier, float duration)
    {
        StopCoroutine(nameof(ResetSpeed));
        currentSpeedMultiplier = multiplier;
        StartCoroutine(ResetSpeed(duration));
    }

    // Reset speed to normal after duration
    private IEnumerator ResetSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentSpeedMultiplier = 1f;
    }

    // Handle player taking damage with knockback
    public void TakeDamage(int amount, Vector2 knockbackForce)
    {
        if (isDead || isKnockedBack || isInvincible) return;

        currentHealth -= amount;
        animator.SetTrigger("HurtTrigger");

        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound, sfxVolume);
        }

        StartCoroutine(ApplyKnockback(knockbackForce));
        StartCoroutine(FlashAndInvincibility());

        if (currentHealth <= 0)
            Die();
    }

    // Apply knockback force and phase through enemies temporarily
    private IEnumerator ApplyKnockback(Vector2 force)
    {
        isKnockedBack = true;
        SetPhaseThrough(true);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
        SetPhaseThrough(false);
    }

    // Flash sprite color and provide invincibility frames
    private IEnumerator FlashAndInvincibility()
    {
        isInvincible = true;

        // Flash sprite multiple times
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        // Additional invincibility time after flashing
        yield return new WaitForSeconds(1.5f - (flashCount * flashDuration * 2));
        isInvincible = false;
    }

    // Handle player death
    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        Debug.Log("[PlayerController2D] Player died.");

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, sfxVolume);
            Debug.Log("[PlayerController2D] Death sound played.");
        }

        LivesSystem livesSystem = FindObjectOfType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.LoseLife();
        }
        else
        {
            Debug.LogWarning("[PlayerController2D] LivesSystem not found.");
        }
    }

    // Reset player state on respawn
    public void ResetOnRespawn()
    {
        isDead = false;
        isKnockedBack = false;
        isInvincible = true;
        currentHealth = maxHealth;

        animator.SetBool("IsDead", false);
        spriteRenderer.color = originalColor;

        StopAllCoroutines();
        StartCoroutine(SpawnInvincibility(2f));
    }

    // Provide temporary invincibility after respawn
    private IEnumerator SpawnInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     Debug.Log($"Colliding with: {collision.gameObject.name}, Layer: {LayerMask.LayerToName(collision.gameObject.layer)}, Contacts: {collision.contactCount}");
    //     for (int i = 0; i < collision.contactCount; i++)
    //     {
    //         Debug.Log($"Contact {i}: Normal: {collision.contacts[i].normal}, Point: {collision.contacts[i].point}");
    //     }
    // }
}
