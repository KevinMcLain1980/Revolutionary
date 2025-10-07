using UnityEngine;
using UnityEngine.SceneManagement;

// Handles level transitions, loading next levels, specific levels, and returning to main menu
public class LevelTransition : MonoBehaviour
{
    // Load the next level in the build settings and save progress
    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there's a next level available
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SaveSystem.SaveGame(nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels! Game complete!");
        }
    }

    // Load a specific level by index and save progress
    public void LoadSpecificLevel(int levelIndex)
    {
        SaveSystem.SaveGame(levelIndex);
        SceneManager.LoadScene(levelIndex);
    }

    // Return to the main menu (scene 0)
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
