using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private Image loadingBar; // Filled image for loading bar
    [SerializeField] private TMP_Text loadingText; // Text showing loading percentage

    [Header("Settings")]
    [SerializeField] private string firstLevelSceneName = "Level 1_Cabin"; // Name of first level to load
    [SerializeField] private float minimumLoadTime = 2f; // Minimum time to display loading screen

    private void Start()
    {
        StartCoroutine(LoadFirstLevel());
    }

    // Load the first level with animated loading bar
    private IEnumerator LoadFirstLevel()
    {
        float elapsedTime = 0f;

        // Start loading the level asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(firstLevelSceneName);
        asyncLoad.allowSceneActivation = false;

        // Update loading bar until minimum time and scene loading complete
        while (elapsedTime < minimumLoadTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;

            // Calculate progress from both actual loading and time elapsed
            float actualProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float timeProgress = elapsedTime / minimumLoadTime;
            float displayProgress = Mathf.Min(actualProgress, timeProgress);

            // Update loading bar fill
            if (loadingBar != null)
                loadingBar.fillAmount = displayProgress;

            // Update loading text percentage
            if (loadingText != null)
                loadingText.text = $"Loading... {Mathf.RoundToInt(displayProgress * 100)}%";

            yield return null;
        }

        // Set to 100% complete
        if (loadingBar != null)
            loadingBar.fillAmount = 1f;

        if (loadingText != null)
            loadingText.text = "Loading... 100%";

        yield return new WaitForSeconds(0.5f);

        // Activate the loaded scene
        asyncLoad.allowSceneActivation = true;
    }
}
