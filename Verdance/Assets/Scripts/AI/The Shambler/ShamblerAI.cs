using UnityEngine;

public class ShamblerAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private bool isChasing = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null || isDead()) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        isChasing = distanceToPlayer <= detectionRange;

        if (isChasing)
        {
            MoveTowardPlayer();
            FacePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // idle
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("Speed", 1f);
    }

    private void FacePlayer()
    {
        float directionToPlayer = playerTransform.position.x - transform.position.x;
        if (Mathf.Abs(directionToPlayer) > 0.1f)
        {
            spriteRenderer.flipX = directionToPlayer < 0f;
        }
        Debug.Log("Facing player. FlipX: " + spriteRenderer.flipX);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"{gameObject.name} has died.");
        // Optional: play death animation, disable AI, destroy object
        Destroy(gameObject);
    }

    private bool isDead()
    {
        return currentHealth <= 0;
    }
}
