using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private bool resetHealthOnRespawn = true;
    [SerializeField] private bool resetSanityOnRespawn = false;
    [SerializeField] private bool resetMagicOnRespawn = false;

    [Header("Lives System")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Input")]
    [SerializeField] private bool allowManualReset = true;

    [Header("Audio")]
    [SerializeField] private AudioClip respawnSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    private Vector3 spawnPoint;
    private bool isDead = false;
    private AudioSource audioSource;

    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;

    private void Start()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);

        if (PlayerSpawnPoint.Instance != null)
        {
            spawnPoint = PlayerSpawnPoint.Instance.GetSpawnPosition();
        }
        else
        {
            spawnPoint = transform.position;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (allowManualReset)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame && keyboard.ctrlKey.isPressed)
            {
                ResetPlayer();
            }
        }
    }

    public void OnPlayerDeath()
    {
        if (isDead) return;

        isDead = true;
        currentLives--;
        OnLivesChanged?.Invoke(currentLives);
        //Debug.Log($"Player died. Remaning Lives: {currentLives}");

        if (currentLives <= 0)
        {
            //Debug.Log("No lives remaning. Game Over.");
            OnGameOver?.Invoke();
        }
        else
        {
            Invoke(nameof(Respawn), respawnDelay);
        }
        
    }

    private void Respawn()
    {
        isDead = false;
        transform.position = spawnPoint;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        PlayerController2D controller = GetComponent<PlayerController2D>();
        if (controller != null)
        {
            controller.ResetOnRespawn();
        }

        ResetPlayerStats();

        if (respawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(respawnSound, sfxVolume);
        }

        //Debug.Log("Player respawned");
    }

    public void ResetPlayer()
    {
        transform.position = spawnPoint;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        ResetPlayerStats();
        //Debug.Log("Player reset to spawn point");
    }

    private void ResetPlayerStats()
    {
        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            if (resetHealthOnRespawn)
            {
                stats.SetHealth(stats.GetMaxHealth());
            }

            if (resetSanityOnRespawn)
            {
                stats.SetSanity(stats.GetMaxSanity());
            }

            if (resetMagicOnRespawn)
            {
                stats.SetMagic(stats.GetMaxMagic());
            }
        }
    }

    public void SetSpawnPoint(Vector3 position)
    {
        spawnPoint = position;
    }

    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;

    public void ResetLives()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
        //Debug.Log($"Lives reset to {currentLives}");
    }
}
