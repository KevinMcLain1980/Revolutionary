using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private bool resetHealthOnRespawn = true;
    [SerializeField] private bool resetSanityOnRespawn = false;
    [SerializeField] private bool resetMagicOnRespawn = false;

    [Header("Input")]
    [SerializeField] private bool allowManualReset = true;

    private Vector3 spawnPoint;
    private bool isDead = false;

    private void Start()
    {
        if (PlayerSpawnPoint.Instance != null)
        {
            spawnPoint = PlayerSpawnPoint.Instance.GetSpawnPosition();
        }
        else
        {
            spawnPoint = transform.position;
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
        Debug.Log("Player died, respawning...");
        Invoke(nameof(Respawn), respawnDelay);
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

        ResetPlayerStats();
        Debug.Log("Player respawned");
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
        Debug.Log("Player reset to spawn point");
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
}
