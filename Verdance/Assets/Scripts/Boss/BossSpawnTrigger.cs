using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private string bossName = "Boss";

    [Header("Music")]
    [SerializeField] private AudioClip bossMusicClip;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;
    private GameObject spawnedBoss;
    private AudioSource musicSource;

    private void Start()
    {
        musicSource = FindFirstObjectByType<AudioSource>();
        if (musicSource == null)
        {
            GameObject musicObject = GameObject.Find("MusicPlayer");
            if (musicObject != null)
            {
                musicSource = musicObject.GetComponent<AudioSource>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            SpawnBoss();
            hasTriggered = true;
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null || spawnPosition == null)
        {
            Debug.LogError("Boss prefab or spawn position not assigned!");
            return;
        }

        spawnedBoss = Instantiate(bossPrefab, spawnPosition.position, spawnPosition.rotation);

        IDamageable bossDamageable = spawnedBoss.GetComponent<IDamageable>();
        if (bossDamageable != null)
        {
            PlayerUI playerUI = FindFirstObjectByType<PlayerUI>();
            if (playerUI != null)
            {
                float maxHealth = bossDamageable.GetMaxHealth();
                playerUI.ShowBossHealthBar(bossName, maxHealth);
            }
        }

        if (bossMusicClip != null && musicSource != null)
        {
            musicSource.clip = bossMusicClip;
            musicSource.Play();
        }

        Debug.Log($"Boss '{bossName}' spawned at {spawnPosition.position}");
    }
}
