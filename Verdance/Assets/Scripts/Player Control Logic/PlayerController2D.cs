using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private float currentSpeedMultiplier = 1f;
    [SerializeField] private float SlopeCheckDistance;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;


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


    [Header("Slope Logic")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 colliderSize;
    private CapsuleCollider2D cc;
    private float slopeDownAngle;
    private Vector2 slopeNormalPerp;
    private bool IsOnSlope;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        MovePlayer();
        SlopeCheck();
        UpdateAnimationStates();
        // Debug grounding status
        Debug.Log($"IsGrounded: {IsGrounded()}");

    }

    private void MovePlayer()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
        }
    }

    private void SlopeCheck()
    {
        Vector2 CheckPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckVertical(CheckPos);
        SlopeCheckHorizontal(CheckPos);
    }

    private void SlopeCheckHorizontal(Vector2 CheckPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(CheckPos, transform.right, SlopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(CheckPos, -transform.right, SlopeCheckDistance, groundLayer);

        if(slopeHitFront)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            IsOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            IsOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 CheckPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(CheckPos, Vector2.down, SlopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                IsOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    private void UpdateAnimationStates()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (moveInput.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (moveInput.x < -0.1f)
            spriteRenderer.flipX = true;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (IsGrounded() && !IsOnSlope)
        {
            // Standard grounded movement
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
            Debug.Log("Moving on flat ground");
        }
        else if (IsGrounded() && IsOnSlope)
        {
            // Slope-adjusted movement
            Vector2 slopeMoveDirection = slopeNormalPerp * -moveInput.x;
            rb.linearVelocity = new Vector2(slopeMoveDirection.x * moveSpeed * currentSpeedMultiplier, rb.linearVelocity.y);
            Debug.Log("Moving on slope");
        }
        else if (!IsGrounded())
        {
            // Airborne movement (optional: limited control)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * currentSpeedMultiplier * 0.5f, rb.linearVelocity.y);
            Debug.Log("Airborne movement");
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("IsJumping");
            Debug.Log("Jump triggered");
        }
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
