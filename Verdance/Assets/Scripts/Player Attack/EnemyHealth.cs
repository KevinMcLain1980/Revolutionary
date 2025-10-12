using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float knockbackResistance = 1f;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    private float currentHealth;
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

        if (knockbackDirection != Vector2.zero && rb != null)
            rb.AddForce(knockbackDirection / knockbackResistance, ForceMode2D.Impulse);

        if (currentHealth <= 0)
            Die();
        else
            OnDamageTaken();
    }

    protected virtual void OnDamageTaken()
    {
        GetComponent<ShamblerAI>()?.OnHurt();
        animator?.SetTrigger("HurtTrigger");

        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, transform.position, sfxVolume);
    }

    protected virtual void Die()
    {
        isDead = true;

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, sfxVolume);

        LevelManager.Instance?.OnEnemyKilled(gameObject);

        ShamblerAI shambler = GetComponent<ShamblerAI>();
        if (shambler != null)
            shambler.Die();
        else
            Destroy(gameObject, 2f);
    }

    public bool IsDead() => isDead;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
