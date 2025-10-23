using UnityEngine;
using System.Collections;

// AI controller for the Shambler enemy that chases and attacks the player
public class ShamblerAI : MonoBehaviour
{
    // Movement configuration
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f; // Speed when chasing player
    [SerializeField] private float attackCooldown = 1.5f; // Time between attacks
    [SerializeField] private float detectionRange = 8f; // Distance to start chasing
    [SerializeField] private float stopChaseRange = 12f; // Distance to stop chasing
    [SerializeField] private float separationDistance = 1f; // Minimum distance from other shamblers
    [SerializeField] private float separationForce = 1.5f; // Force applied to separate from others
    private float lastAttackTime = 0f; // Timestamp of last attack

    // Component and state references
    private Transform player; // Reference to player transform
    private Rigidbody2D rb; // Rigidbody for physics movement
    private bool isStunned = false; // Whether shambler is stunned
    private bool isDead = false; // Whether shambler is dead
    private bool isChasing = false; // Whether shambler is actively chasing player
    private bool isInvincible = false; // Whether shambler is invincible

    // Combat configuration
    [Header("Combat")]
    [SerializeField] private float meleeDamage = 15f; // Damage dealt per attack
    [SerializeField] private float playerKnockbackImpulse = 10f; // Knockback force applied to player

    // Damage Feedback
    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Sprite renderer for color flashing
    [SerializeField] private Color flashColor = Color.red; // Color to flash when damaged
    [SerializeField] private float flashDuration = 0.1f; // Duration of each flash
    [SerializeField] private int flashCount = 3; // Number of flashes when damaged
    [SerializeField] private float knockbackDuration = 0.3f; // Duration of knockback state

    // Animation configuration
    [Header("Animation")]
    [SerializeField] private Animator animator; // Reference to animator component

    // Initialize components on awake
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Find player reference on start
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Main update loop for AI behavior
    private void Update()
    {
        // Skip if stunned, dead, or no player found
        if (isStunned || isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Start chasing if player enters detection range
        if (!isChasing && distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        // Stop chasing if player moves too far away
        else if (isChasing && distanceToPlayer > stopChaseRange)
        {
            isChasing = false;
        }
    }

    // Physics updates in FixedUpdate to prevent sticking issues
    private void FixedUpdate()
    {
        // Skip if stunned, dead, or no player found
        if (isStunned || isDead || player == null) return;

        // Execute chase behavior or idle
        if (isChasing)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Stop moving and set idle animation
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }
    }

    // Move shambler toward the player's position
    private void MoveTowardsPlayer()
    {
        // Calculate direction and apply velocity
        Vector2 direction = (player.position - transform.position).normalized;

        // Apply separation from other shamblers to prevent clustering
        Vector2 separation = CalculateSeparation();
        direction += separation;
        direction.Normalize();

        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Face the player during pursuit
        FacePlayer();

        // Update animation based on movement speed
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    // Calculate separation force to avoid overlapping with other shamblers
    private Vector2 CalculateSeparation()
    {
        Vector2 separationVector = Vector2.zero;
        int nearbyCount = 0;

        // Find all nearby shamblers
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == gameObject) continue; // Skip self

            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            // If too close, push away
            if (distance < separationDistance && distance > 0)
            {
                Vector2 awayDirection = (transform.position - enemy.transform.position).normalized;
                separationVector += awayDirection / distance; // Stronger push when closer
                nearbyCount++;
            }
        }

        // Average and scale the separation force
        if (nearbyCount > 0)
        {
            separationVector /= nearbyCount;
            separationVector *= separationForce;
        }

        return separationVector;
    }

    // Flip the shambler sprite to face the player
    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = player.position - transform.position;

        // Face right if player is to the right
        if (directionToPlayer.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        // Face left if player is to the left
        else if (directionToPlayer.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Called when shambler takes damage
    public void OnHurt()
    {
        if (isDead || isInvincible) return;

        // Trigger hurt animation
        if (animator != null)
        {
            animator.SetTrigger("HurtTrigger");
        }

        // Stun the shambler briefly
        StartCoroutine(StunForSeconds(0.5f));
        StartCoroutine(Flash());
    }

    // Flash sprite color and provide invincibility frames
    private IEnumerator Flash()
    {
        

        Color originalColor = spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        yield return new WaitForSeconds(1.5f - (flashCount * flashDuration * 2));
        
    }

    // Apply knockback force to the shambler
    private IEnumerator ApplyKnockback(Vector2 force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
    }

    // Stun the shambler for a specified duration
    private IEnumerator StunForSeconds(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    // Handle shambler death
    public void Die()
    {
        isDead = true;

        // Set death animation if parameter exists
        if (animator != null && animator.parameters != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == "IsDead")
                {
                    animator.SetBool("IsDead", true);
                    break;
                }
            }
        }

        // Stop movement and disable script
        rb.linearVelocity = Vector2.zero;
        enabled = false;

        // Hide sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Destroy game object after brief delay
        Destroy(gameObject, 0.5f);
    }

    // Check if shambler is dead
    public bool IsDead() => isDead;

    // Handle collision with player - initial contact
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Face the player during attack
            FacePlayer();

            // Attack if cooldown has elapsed
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Deal damage to player stats
                PlayerStats playerStats = PlayerStats.Instance;
                if (playerStats != null)
                {
                    playerStats.TakeDamage(meleeDamage);
                }

                // Apply knockback to player
                PlayerController2D playerController = collision.gameObject.GetComponent<PlayerController2D>();
                if (playerController != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerController.TakeDamage((int)meleeDamage, knockbackDirection * playerKnockbackImpulse);
                }

                animator.SetTrigger("AttackTrigger");
                lastAttackTime = Time.time;
                //Debug.Log($"Shambler dealt {meleeDamage} damage to player");
            }
        }
    }

    // Handle sustained collision with player - continuous contact
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Keep facing the player during sustained contact
            FacePlayer();

            // Attack if cooldown has elapsed
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Deal damage to player stats
                PlayerStats playerStats = PlayerStats.Instance;
                if (playerStats != null)
                {
                    playerStats.TakeDamage(meleeDamage);
                }

                // Apply knockback to player
                PlayerController2D playerController = collision.gameObject.GetComponent<PlayerController2D>();
                if (playerController != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerController.TakeDamage((int)meleeDamage, knockbackDirection * playerKnockbackImpulse);
                }

                lastAttackTime = Time.time;
                //Debug.Log($"Shambler dealt {meleeDamage} damage to player");
            }
        }
    }
}
