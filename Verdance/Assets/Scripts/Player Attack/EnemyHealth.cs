using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackResistance = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;
    // SFX volume controlled by SettingsManager

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            GameObject sfxObject = GameObject.Find("SFX");
            if (sfxObject != null)
            {
                audioSource = sfxObject.GetComponent<AudioSource>();
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        //Debug.Log($"{gameObject.name} initialized with {currentHealth}/{maxHealth} health");
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection = default)
    {
        if (isDead) return;

        currentHealth -= damage;
        //Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

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

        if (hurtSound != null && audioSource != null)
        {
            audioSource.volume = 1f;
            float volume = SettingsManager.Instance != null && SettingsManager.Instance.HasAudioMixer() ? 1f : (SettingsManager.Instance?.GetSFXVolume() ?? 0.8f);
            audioSource.PlayOneShot(hurtSound, volume);
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        //Debug.Log($"{gameObject.name} died!");

        if (deathSound != null && audioSource != null)
        {
            audioSource.volume = 1f;
            float volume = SettingsManager.Instance != null && SettingsManager.Instance.HasAudioMixer() ? 1f : (SettingsManager.Instance?.GetSFXVolume() ?? 0.8f);
            audioSource.PlayOneShot(deathSound, volume);
        }

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
