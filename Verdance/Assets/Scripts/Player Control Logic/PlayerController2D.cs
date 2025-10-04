using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private GameObject thornbrandHitboxPrefab;
    [SerializeField] private Transform attackSpawnPoint;
    [SerializeField] private Animator animator;
    private bool canAttack = true;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 movement = new Vector2(moveInput.x, rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, movement.y);
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

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

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
}
