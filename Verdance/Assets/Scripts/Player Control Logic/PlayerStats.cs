using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    [Header("Sanity")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity;
    [SerializeField] private float sanityRegenRate = 5f;

    [Header("Magic")]
    [SerializeField] private float maxMagic = 100f;
    [SerializeField] private float currentMagic;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnSanityChanged;
    public event Action<float, float> OnMagicChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentSanity = maxSanity;
        currentMagic = maxMagic;
    }

    private void Update()
    {
        RegenerateSanity();
    }

    private void RegenerateSanity()
    {
        if (currentSanity < maxSanity)
        {
            SetSanity(currentSanity + sanityRegenRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        SetHealth(currentHealth - damage);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (hurtSound != null)
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position, sfxVolume);
        }
    }

    public void Heal(float amount)
    {
        SetHealth(currentHealth + amount);
    }

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetSanity(float value)
    {
        currentSanity = Mathf.Clamp(value, 0, maxSanity);
        OnSanityChanged?.Invoke(currentSanity, maxSanity);
    }

    public void SetMagic(float value)
    {
        currentMagic = Mathf.Clamp(value, 0, maxMagic);
        OnMagicChanged?.Invoke(currentMagic, maxMagic);
    }

    public bool ConsumeMagic(float amount)
    {
        if (currentMagic >= amount)
        {
            SetMagic(currentMagic - amount);
            return true;
        }
        return false;
    }

    public void LoseSanity(float amount)
    {
        SetSanity(currentSanity - amount);
    }

    public void RestoreSanity(float amount)
    {
        SetSanity(currentSanity + amount);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentSanity() => currentSanity;
    public float GetMaxSanity() => maxSanity;
    public float GetCurrentMagic() => currentMagic;
    public float GetMaxMagic() => maxMagic;

    private void Die()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, sfxVolume);
        }

        Debug.Log("Player died!");

        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.OnPlayerDeath();
        }
    }
}
