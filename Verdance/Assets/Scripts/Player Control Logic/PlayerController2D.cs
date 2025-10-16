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
    private bool isAttacking = false;

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

        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimationStates();
    }

    private bool IsInState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    private bool IsForbiddenState()
    {
        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);

        if (animator.IsInTransition(0))
        {
            AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(0);
            return next.IsName("Jump") || next.IsName("Run") || next.IsName("Dead");
        }

        return current.IsName("Jump") || current.IsName("Run") || current.IsName("Dead");
    }

    public void SetPhaseThrough(bool value)
    {
        gameObject.layer = value ? LayerMask.NameToLayer("Phasing") : LayerMask.NameToLayer("Player");
        animator.SetBool("IsPhasing", value);
    }

    private void MovePlayer()
    {
        if (isKnockedBack || isAttacking) return;
        Vector2 newVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }

    private void UpdateAnimationStates()
    {
        if (isAttacking && !IsInState("Jump") && !IsInState("Dead")) return;

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", IsGrounded());

        if (moveInput.x > 0.1f) spriteRenderer.flipX = false;
        else if (moveInput.x < -0.1f) spriteRenderer.flipX = true;
    }

    public void OnMove(InputValue value)
    {
        if (isKnockedBack || isAttacking) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded() && !IsInState("Attack") && !IsInState("Hurt"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("JumpTrigger");
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !IsForbiddenState())
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
        if (!canAttack || isAttacking || IsForbiddenState()) return;

        isAttacking = true;
        animator.SetTrigger("AttackTrigger");
        canAttack = false;

        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke(nameof(ResetAttack), attackCooldown);
        Invoke(nameof(ResetAttackLockout), attackDuration);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void ResetAttackLockout()
    {
        isAttacking = false;
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
        if (isDead || isKnockedBack || isInvincible || IsInState("Dead")) return;

        currentHealth -= amount;

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
        isKnockedBack = true;
        isAttacking = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // Optional: switch to a non-collidable layer to avoid re-triggering
        gameObject.layer = LayerMask.NameToLayer("Phasing");

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        yield return new WaitForSeconds(0.2f); // brief pause before regaining control

        gameObject.layer = LayerMask.NameToLayer("Player");

        isInvincible = false;
        isKnockedBack = false;
        isAttacking = false;
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

        // Reset movement and attack lockouts
        isAttacking = false;
        canAttack = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator SpawnInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spriteRenderer.bounds.center, spriteRenderer.bounds.size);
        }

        if (thornbrandHitbox != null)
        {
            BoxCollider2D hitboxCollider = thornbrandHitbox.GetComponent<BoxCollider2D>();
            if (hitboxCollider != null)
            {
                Gizmos.color = Color.red;
                Vector3 hitboxWorldPos = thornbrandHitbox.transform.position;
                Vector3 hitboxSize = hitboxCollider.size;
                Gizmos.DrawWireCube(hitboxWorldPos, hitboxSize);
            }
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
