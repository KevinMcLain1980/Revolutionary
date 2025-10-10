using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private string nextLevelName;
    [SerializeField] private bool requireBossKill = true;
    [SerializeField] private bool requireAllEnemiesKilled = false;
    [SerializeField] private float levelCompleteDelay = 2f;

    [Header("References")]
    [SerializeField] private GameObject levelCompleteUI;

    private HashSet<GameObject> remainingEnemies = new HashSet<GameObject>();
    private GameObject boss;
    private bool bossKilled = false;
    private bool levelComplete = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(false);

        RegisterAllEnemies();
    }

    private void RegisterAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            remainingEnemies.Add(enemy);
        }

        Debug.Log($"Level started with {remainingEnemies.Count} enemies");
    }

    public void RegisterBoss(GameObject bossObject)
    {
        boss = bossObject;
        Debug.Log($"Boss registered: {boss.name}");
    }

    public void OnEnemyKilled(GameObject enemy)
    {
        if (remainingEnemies.Contains(enemy))
        {
            remainingEnemies.Remove(enemy);
            Debug.Log($"Enemy killed. Remaining: {remainingEnemies.Count}");
            CheckLevelCompletion();
        }
    }

    public void OnBossKilled(GameObject bossObject)
    {
        if (bossObject == boss)
        {
            bossKilled = true;
            Debug.Log("Boss killed!");
            CheckLevelCompletion();
        }
    }

    private void CheckLevelCompletion()
    {
        if (levelComplete) return;

        bool bossConditionMet = !requireBossKill || bossKilled;
        bool enemiesConditionMet = !requireAllEnemiesKilled || remainingEnemies.Count == 0;

        if (bossConditionMet && enemiesConditionMet)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        levelComplete = true;
        Debug.Log("Level Complete!");

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        SaveGame();
        Invoke(nameof(LoadNextLevel), levelCompleteDelay);
    }

    private void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            currentLevel = SceneManager.GetActiveScene().name,
            nextLevel = nextLevelName,
            playerHealth = PlayerStats.Instance?.GetCurrentHealth() ?? 100f,
            playerSanity = PlayerStats.Instance?.GetCurrentSanity() ?? 100f,
            playerMagic = PlayerStats.Instance?.GetCurrentMagic() ?? 100f,
            saveTime = System.DateTime.Now.ToString()
        };

        SaveSystem.SaveGame(saveData);
        Debug.Log("Game saved!");
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("No next level specified!");
        }
    }

    public void ForceCompleteLevel()
    {
        CompleteLevel();
    }
}
