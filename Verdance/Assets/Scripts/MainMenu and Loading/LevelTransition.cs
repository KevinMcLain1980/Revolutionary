using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            GameSaveData saveData = new GameSaveData
            {
                currentLevel = SceneManager.GetSceneByBuildIndex(nextSceneIndex).name,
                nextLevel = nextSceneIndex + 1 < SceneManager.sceneCountInBuildSettings ?
                    SceneManager.GetSceneByBuildIndex(nextSceneIndex + 1).name : "",
                playerHealth = PlayerStats.Instance?.GetCurrentHealth() ?? 100f,
                playerSanity = PlayerStats.Instance?.GetCurrentSanity() ?? 100f,
                playerMagic = PlayerStats.Instance?.GetCurrentMagic() ?? 100f,
                saveTime = System.DateTime.Now.ToString(),
                levelsCompleted = currentSceneIndex
            };

            SaveSystem.SaveGame(saveData);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels! Game complete!");
        }
    }

    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            GameSaveData saveData = new GameSaveData
            {
                currentLevel = SceneManager.GetSceneByBuildIndex(levelIndex).name,
                nextLevel = levelIndex + 1 < SceneManager.sceneCountInBuildSettings ?
                    SceneManager.GetSceneByBuildIndex(levelIndex + 1).name : "",
                playerHealth = PlayerStats.Instance?.GetCurrentHealth() ?? 100f,
                playerSanity = PlayerStats.Instance?.GetCurrentSanity() ?? 100f,
                playerMagic = PlayerStats.Instance?.GetCurrentMagic() ?? 100f,
                saveTime = System.DateTime.Now.ToString(),
                levelsCompleted = levelIndex - 1
            };

            SaveSystem.SaveGame(saveData);
            SceneManager.LoadScene(levelIndex);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
