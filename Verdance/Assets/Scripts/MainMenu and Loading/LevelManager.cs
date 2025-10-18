using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private Object nextLevelScene;
    [SerializeField] private bool requireBossKill = false;
    [SerializeField] private bool requireAllEnemiesKilled = true;
    [SerializeField] private bool requireKeyUsed = false;
    [SerializeField] private float levelCompleteDelay = 2f;

    [Header("References")]
    [SerializeField] private GameObject levelCompleteUI;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySFX;
    [SerializeField] private float victoryPlayDuration = 7f;
    [SerializeField] private float victoryFadeOutTime = 2f;
    [SerializeField] private AudioSource levelMusicSource;

    private HashSet<GameObject> remainingEnemies = new HashSet<GameObject>();
    private GameObject boss;
    private bool bossKilled = false;
    private bool keyUsed = false;
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

        //Debug.Log($"Level started with {remainingEnemies.Count} enemies");
    }

    public void RegisterBoss(GameObject bossObject)
    {
        boss = bossObject;
        //Debug.Log($"Boss registered: {boss.name}");
    }

    public void OnEnemyKilled(GameObject enemy)
    {
        if (remainingEnemies.Contains(enemy))
        {
            remainingEnemies.Remove(enemy);
            //Debug.Log($"Enemy killed. Remaining: {remainingEnemies.Count}");
            CheckLevelCompletion();
        }
    }

    public void OnBossKilled(GameObject bossObject)
    {
        if (bossObject == boss)
        {
            bossKilled = true;
            //Debug.Log("Boss killed!");
            CheckLevelCompletion();
        }
    }

    public bool CheckLevelCompletion()
    {
        if (levelComplete) return true;

        bool bossConditionMet = !requireBossKill || bossKilled;
        bool enemiesConditionMet = !requireAllEnemiesKilled || remainingEnemies.Count == 0;
        bool keyConditionMet = !requireKeyUsed || keyUsed;
        return bossConditionMet && enemiesConditionMet && keyConditionMet;
    }

    public bool CheckEnemiesAndBossDefeated()
    {
        bool bossConditionMet = !requireBossKill || bossKilled;
        bool enemiesConditionMet = !requireAllEnemiesKilled || remainingEnemies.Count == 0;
        return bossConditionMet && enemiesConditionMet;
    }

    public int GetRemainingEnemyCount()
    {
        return remainingEnemies.Count;
    }
    public void OnKeyUsed()
    {
        keyUsed = true;
        //Debug.Log("Key used on door!");
        CheckLevelCompletion();
    }

    private void CompleteLevel()
    {
        levelComplete = true;
        //Debug.Log("Level Complete!");

        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);

        if (levelMusicSource != null)
        {
            levelMusicSource.Stop();
            levelMusicSource.enabled = false;
        }

        if (victorySFX != null && audioSource != null)
        {
            audioSource.clip = victorySFX;
            audioSource.Play();
            StartCoroutine(FadeOutAudio(audioSource, victoryPlayDuration, victoryFadeOutTime));
        }

        SaveGame();
        Invoke(nameof(LoadNextLevel), levelCompleteDelay);
    }

    private System.Collections.IEnumerator FadeOutAudio(AudioSource source, float playDuration, float fadeOutTime)
    {
        float startVolume = source.volume;

        yield return new WaitForSeconds(playDuration);

        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutTime);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private void SaveGame()
    {
        string nextLevelName = nextLevelScene != null ? nextLevelScene.name : "";

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
        //Debug.Log("Game saved!");
    }

    private void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            //Debug.Log($"Loading scene index:  {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            //Debug.LogWarning("No next scene in build settings");
        }

    }

    public void ForceCompleteLevel()
    {
        CompleteLevel();
    }
}
