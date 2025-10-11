using UnityEngine;
using System.Collections;

public class ShamblerAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private bool isStunned = false;
    private bool isDead = false;

    [Header("Combat")]
    [SerializeField] private float meleeDamage = 15f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isStunned || isDead || player == null) return;
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    public void OnHurt()
    {
        if (isDead) return;

        if (animator != null)
        {
            animator.SetTrigger("HurtTrigger");
        }

        StartCoroutine(StunForSeconds(0.5f));
    }

    private IEnumerator ApplyKnockback(Vector2 force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator StunForSeconds(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    public void Die()
    {
        isDead = true;

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

        rb.linearVelocity = Vector2.zero;
        enabled = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        Destroy(gameObject, 0.5f);
    }

    public bool IsDead() => isDead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PlayerStats playerStats = PlayerStats.Instance;
                if (playerStats != null)
                {
                    playerStats.TakeDamage(meleeDamage);
                }

                PlayerController2D playerController = collision.gameObject.GetComponent<PlayerController2D>();
                if (playerController != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerController.TakeDamage((int)meleeDamage, knockbackDirection);
                }

                lastAttackTime = Time.time;
                Debug.Log($"Shambler dealt {meleeDamage} damage to player");
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerStats playerStats = PlayerStats.Instance;
            if (playerStats != null)
            {
                playerStats.TakeDamage(meleeDamage);
            }

            PlayerController2D playerController = collision.gameObject.GetComponent<PlayerController2D>();
            if (playerController != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerController.TakeDamage((int)meleeDamage, knockbackDirection);
            }

            lastAttackTime = Time.time;
            Debug.Log($"Shambler dealt {meleeDamage} damage to player");
        }
    }
}
