using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

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
        Vector2 movement = new Vector2(moveInput.x, 0f) * moveSpeed;
        rb.linearVelocity = new Vector2(movement.x, rb.linearVelocity.y);
    }

    // Called by Input System (Send Messages)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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

        animator.SetTrigger("Attack");
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
