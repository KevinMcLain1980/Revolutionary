using UnityEngine;

public class ShamblerAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float idleDirectionChangeInterval = 2f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private bool isChasing = false;
    private float idleTimer = 0f;
    private int idleDirection = 1; // 1 = right, -1 = left

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

        ChooseNewIdleDirection();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null || isDead()) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        isChasing = distanceToPlayer <= detectionRange;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            PatrolIdle();
        }
    }

    private void PatrolIdle()
    {
        idleTimer += Time.fixedDeltaTime;

        if (idleTimer >= idleDirectionChangeInterval)
        {
            ChooseNewIdleDirection();
            idleTimer = 0f;
        }

        rb.linearVelocity = new Vector2(idleDirection * moveSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = idleDirection < 0f;
    }

    private void ChooseNewIdleDirection()
    {
        idleDirection = Random.value < 0.5f ? -1 : 1;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = direction.x < 0f;
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
        Destroy(gameObject);
    }

    private bool isDead()
    {
        return currentHealth <= 0;
    }
}
