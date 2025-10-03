using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed")]
    public float moveSpeed = 5f;

    [Tooltip("Vertical jump force")]
    public float jumpForce = 12f;

    [Header("Ground Detection")]
    [Tooltip("Transform used to check if player is grounded")]
    public Transform groundCheck;

    [Tooltip("Radius of ground check overlap")]
    public float groundCheckRadius = 0.2f;

    [Tooltip("Layer considered as ground")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpQueued;
    private bool isGrounded;
    private bool isCrouching;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
    }

    void FixedUpdate()
    {
        CheckGrounded();
    }

    // Input System callbacks (Send Messages mode)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !isCrouching)
        {
            jumpQueued = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValue<float>() > 0;
    }

    // Movement logic
    void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (jumpQueued)
        {
            rb.linearVelocity = new Vector2(moveInput.x, jumpForce);
            jumpQueued = false;
        }
    }

    void HandleCrouch()
    {
        // Optional: Adjust visuals or collider here
        Debug.Log("Crouching: " + isCrouching);
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            Debug.LogWarning("GroundCheck transform not assigned in PlayerController2D.");
        }
    }
}
