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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
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
        if (value.isPressed && IsGrounded() && !isKnockedBack)
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
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (!canAttack || isKnockedBack) return;
        animator.SetTrigger("AttackTrigger");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack() => canAttack = true;

    public void ActivateThornbrandHitbox()
    {
        if (thornbrandHitbox != null)
        {
            thornbrandHitbox.SetActive(true);
            StartCoroutine(DisableHitboxAfterDelay(0.2f));
        }
    }

    private IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        thornbrandHitbox.SetActive(false);
    }

    public void OnCastWindStep(InputValue value)
    {
        if (value.isPressed && canCastWindStep && !isKnockedBack)
        {
            magicManager?.CastSpell("WindStep");
            canCastWindStep = false;
            Invoke(nameof(ResetWindStep), windStepCooldown);
        }
    }

    public void OnCastLightPulse(InputValue value)
    {
        if (value.isPressed && canCastLightPulse && !isKnockedBack)
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

    public void TakeDamage(int amount, Vector2 _unused)
    {
        if (isDead || isKnockedBack || isInvincible) return;

        currentHealth -= amount;
        animator.SetTrigger("HurtTrigger");

        StartCoroutine(FlashAndRecover());

        if (currentHealth <= 0)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            TakeDamage(1, Vector2.zero); // Knockback handled internally
        }
    }

    private IEnumerator FlashAndRecover()
    {
        isKnockedBack = true;
        isInvincible = true;

        Vector2 reverseDirection = -moveInput.normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(reverseDirection * moveSpeed, ForceMode2D.Impulse);

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        yield return new WaitForSeconds(0.5f);

        isKnockedBack = false;
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        // Disable movement, input, etc.
    }
}
