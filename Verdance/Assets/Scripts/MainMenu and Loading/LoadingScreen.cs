using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LoadingScreen : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private Image loadingBar; // Filled image for loading bar
    [SerializeField] private TMP_Text loadingText; // Text showing loading percentage
    [SerializeField] private TMP_Text controlsText; // Text for animated controls display

    [Header("Settings")]
    [SerializeField] private string firstLevelSceneName = "Level 1_Cabin"; // Name of first level to load
    [SerializeField] private float minimumLoadTime = 2f; // Minimum time to display loading screen

    private bool isControlsAnimationDone = false;

    private void Start()
    {
        StartCoroutine(LoadFirstLevel());
        if (controlsText != null)
        {
            StartCoroutine(AnimateControls());
        }
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

            // Cap progress at 95% until controls animation completes one cycle
            if (!isControlsAnimationDone)
            {
                displayProgress = Mathf.Min(displayProgress, 0.95f);
            }

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

    // Animate controls text with fade in/out effect
    private IEnumerator AnimateControls()
    {
        string[] controlsMessages = new string[]
        {
            "Use A and D Keys to Move, But these can be changed in settings",
            "Press Space to Jump",
            "Press Left CTRL to Attack/Use items",
            "Press 1-3 for Inventory",
            "The Shambler is a demonic creature who has taken over the forest.",
            "Tip: In order to complete each level kill all enemies, find the key and use it on the chest"
        };

        int index = 0;
        CanvasGroup canvasGroup = controlsText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = controlsText.gameObject.AddComponent<CanvasGroup>();
        }

        while (true)
        {
            string message = controlsMessages[index];
            controlsText.text = "";
            canvasGroup.alpha = 1f; // Ensure visible for typing
            controlsText.color = new Color(0x93 / 255f, 0xA1 / 255f, 0x83 / 255f); // Set to specified hex color

            // Type out the message
            foreach (char c in message)
            {
                controlsText.text += c;
                yield return new WaitForSeconds(0.05f); // Typing speed
            }

            // Display duration
            yield return new WaitForSeconds(2f);

            // Fade out
            float time = 0f;
            float fadeDuration = 0.5f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
                yield return null;
            }

            index = (index + 1) % controlsMessages.Length;
            if (index == 0)
            {
                isControlsAnimationDone = true;
            }
        }
    }
}
