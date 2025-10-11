using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackResistance = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection = default)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (knockbackDirection != Vector2.zero && rb != null)
        {
            rb.AddForce(knockbackDirection / knockbackResistance, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            OnDamageTaken();
        }
    }

    protected virtual void OnDamageTaken()
    {
        ShamblerAI shambler = GetComponent<ShamblerAI>();
        if (shambler != null)
        {
            shambler.OnHurt();
        }

        if (animator != null)
        {
            animator.SetTrigger("HurtTrigger");
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} died!");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnEnemyKilled(gameObject);
        }

        ShamblerAI shambler = GetComponent<ShamblerAI>();
        if (shambler != null)
        {
            shambler.Die();
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }

    public bool IsDead() => isDead;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
