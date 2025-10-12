using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private float currentSpeedMultiplier = 1f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack")]
    [SerializeField] private GameObject thornbrandHitbox;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Animator animator;
    private bool canAttack = true;

    [Header("Magic")]
    [SerializeField] private MagicManager magicManager;
    [SerializeField] private float windStepCooldown = 5f;
    [SerializeField] private float lightPulseCooldown = 8f;
    private bool canCastWindStep = true;
    private bool canCastLightPulse = true;

    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Color originalColor;
    private bool isKnockedBack = false;
    private bool isInvincible = false;
    private bool isDead = false;
    private int currentHealth;
    [SerializeField] private int maxHealth = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimationStates();
    }

    public void SetPhaseThrough(bool value)
    {
        gameObject.layer = value ? LayerMask.NameToLayer("Phasing") : LayerMask.NameToLayer("Player");
        animator.SetBool("IsPhasing", value);
    }

    private void MovePlayer()
    {
        if (isKnockedBack) return;
        Vector2 newVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }

    private void UpdateAnimationStates()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        if (moveInput.x > 0.1f) spriteRenderer.flipX = false;
        else if (moveInput.x < -0.1f) spriteRenderer.flipX = true;
    }

    public void OnMove(InputValue value)
    {
        if (isKnockedBack) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("IsJumping");
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

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

    private void TryAttack()
    {
        if (!canAttack) return;
        animator.SetTrigger("AttackTrigger");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    public void ActivateThornbrandHitbox()
    {
        if (thornbrandHitbox != null)
        {
            float direction = spriteRenderer.flipX ? -1 : 1;
            Vector3 localPos = thornbrandHitbox.transform.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * direction;
            thornbrandHitbox.transform.localPosition = localPos;

            thornbrandHitbox.SetActive(true);
            StartCoroutine(DisableHitboxAfterDelay(0.2f));
        }
    }

    private IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        thornbrandHitbox.SetActive(false);
    }

    public void UseInventoryItem(int slotIndex)
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.UseItem(slotIndex);
        }
    }

    public void OnCastWindStep(InputValue value)
    {
        if (value.isPressed && canCastWindStep)
        {
            magicManager?.CastSpell("WindStep");
            canCastWindStep = false;
            Invoke(nameof(ResetWindStep), windStepCooldown);
        }
    }

    public void OnCastLightPulse(InputValue value)
    {
        if (value.isPressed && canCastLightPulse)
        {
            magicManager?.CastSpell("LightPulse");
            canCastLightPulse = false;
            Invoke(nameof(ResetLightPulse), lightPulseCooldown);
        }
    }

    private void ResetWindStep() => canCastWindStep = true;
    private void ResetLightPulse() => canCastLightPulse = true;

    public void ModifySpeed(float multiplier) => currentSpeedMultiplier = multiplier;

    public void ModifySpeed(float multiplier, float duration)
    {
        StopCoroutine(nameof(ResetSpeed));
        currentSpeedMultiplier = multiplier;
        StartCoroutine(ResetSpeed(duration));
    }

    private IEnumerator ResetSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentSpeedMultiplier = 1f;
    }

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

    private IEnumerator ApplyKnockback(Vector2 force)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    private IEnumerator FlashAndInvincibility()
    {
        isInvincible = true;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        yield return new WaitForSeconds(1.5f - (flashCount * flashDuration * 2));
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound, sfxVolume);
        }

        // Disable movement, input, etc.
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
