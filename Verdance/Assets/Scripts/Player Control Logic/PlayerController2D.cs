using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Slope Detection")]
    [SerializeField] private float slopeRayLength = 0.5f;
    [SerializeField] private float currentSlopeAngle;
    private bool isOnSlope;
    private Vector2 slopeDirection;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private GameObject thornbrandHitboxPrefab;
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private Animator animator;
    private bool canAttack = true;

    [Header("Magic")]
    [SerializeField] private MagicManager magicManager;
    [SerializeField] private float windStepCooldown = 5f;
    [SerializeField] private float lightPulseCooldown = 8f;
    private bool canCastWindStep = true;
    private bool canCastLightPulse = true;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimationStates();
    }

    private void MovePlayer()
    {
        Vector2 moveDirection = IsGrounded() ? GetSlopeAdjustedDirection() : new Vector2(moveInput.x, rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
    }

    private Vector2 GetSlopeAdjustedDirection()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, slopeRayLength, groundLayer);
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            currentSlopeAngle = Vector2.Angle(normal, Vector2.up);
            slopeDirection = new Vector2(normal.y, -normal.x); // perpendicular to normal
            isOnSlope = currentSlopeAngle > 0f;

            animator.SetFloat("SlopeAngle", currentSlopeAngle); // optional animator hook
            return slopeDirection.normalized * moveInput.x;
        }

        isOnSlope = false;
        currentSlopeAngle = 0f;
        return new Vector2(moveInput.x, 0f); // fallback: flat ground
    }

    private void UpdateAnimationStates()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsJumping", !IsGrounded());

        if (moveInput.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (moveInput.x < -0.1f)
            spriteRenderer.flipX = true;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        if (!canAttack) return;

        animator.SetTrigger("AttackTrigger");
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    // Called by animation event
    public void SpawnThornbrandHitbox()
    {
        if (thornbrandHitboxPrefab != null && attackSpawnPoint != null)
        {
            Instantiate(thornbrandHitboxPrefab, attackSpawnPoint.position, Quaternion.identity);
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

    public void SetPhaseThrough(bool value)
    {
        Debug.Log($"Phase-through set to {value}");
        // Optional: implement collision layer toggling here
    }
}
