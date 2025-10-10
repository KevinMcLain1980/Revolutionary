using UnityEngine;
using System.Collections;

public class ShamblerAI : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    private Transform player;
    private Rigidbody2D rb;
    private bool isStunned = false;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isStunned || player == null) return;
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    public void TakeDamage(int amount, Vector2 knockbackForce)
    {
        currentHealth -= amount;
        animator.SetTrigger("HurtTrigger");

        StartCoroutine(ApplyKnockback(knockbackForce));
        StartCoroutine(StunForSeconds(1f));

        if (currentHealth <= 0)
            Die();
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

    private void Die()
    {
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        // Disable AI, collider, or trigger death effects here
    }
}
