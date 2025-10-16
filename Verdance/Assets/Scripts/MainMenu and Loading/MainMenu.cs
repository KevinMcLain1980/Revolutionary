using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    // Button references for menu navigation
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    // Settings panel references
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private SettingsMenu settingsMenu;

    // Audio feedback for user interactions
    [Header("Sound Effects")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip exitSound;
    [SerializeField] private AudioSource audioSource;

    // Visual hover effect settings
    [Header("Hover Visual Feedback")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverAnimationSpeed = 0.15f;
    [SerializeField] private Color hoverTintColor = new Color(1f, 1f, 0.7f, 1f);

    // Tracks if a scene transition is in progress
    private bool isTransitioning = false;
    // Stores each button's original visual state for proper animation reset
    private System.Collections.Generic.Dictionary<Button, ButtonState> buttonOriginalStates = new System.Collections.Generic.Dictionary<Button, ButtonState>();
    // Tracks active hover animations per button to prevent conflicts
    private System.Collections.Generic.Dictionary<Button, Coroutine> activeButtonCoroutines = new System.Collections.Generic.Dictionary<Button, Coroutine>();

    private struct ButtonState
    {
        public Color originalColor;
        public Vector3 originalScale;
    }

    private void Start()
    {
        SetupButtons();
        UpdateButtonStates();

        // Hide settings panel on start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Ensure audio source is available
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void SetupButtons()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGame);
            AddHoverSound(newGameButton);
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OnLoadGame);
            AddHoverSound(loadGameButton);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinue);
            AddHoverSound(continueButton);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettings);
            AddHoverSound(settingsButton);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExit);
            AddHoverSound(exitButton);
        }
    }

    private void AddHoverSound(Button button)
    {
        // Store button's original visual state for reset
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = new ButtonState
        {
            originalColor = buttonImage != null ? buttonImage.color : Color.white,
            originalScale = button.transform.localScale
        };
        buttonOriginalStates[button] = originalState;

        // Add or get EventTrigger component
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Register hover enter event
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => {
            OnButtonHoverEnter(button);
        });
        trigger.triggers.Add(hoverEntry);

        // Register hover exit event
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            OnButtonHoverExit(button);
        });
        trigger.triggers.Add(exitEntry);
    }

    private void OnButtonHoverEnter(Button button)
    {
        // Skip if transitioning, button disabled, or menu inactive
        if (isTransitioning || !button.interactable || !gameObject.activeInHierarchy) return;

        PlayHoverSound();

        // Stop any existing animation for this button
        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        // Start hover animation
        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, true));
    }

    private void OnButtonHoverExit(Button button)
    {
        // Skip if transitioning, button disabled, or menu inactive
        if (isTransitioning || !button.interactable || !gameObject.activeInHierarchy) return;

        // Stop any existing animation for this button
        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        // Start exit animation
        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, false));
    }

    private System.Collections.IEnumerator AnimateButtonHover(Button button, bool isHovering)
    {
        if (!buttonOriginalStates.ContainsKey(button)) yield break;

        Transform buttonTransform = button.transform;
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = buttonOriginalStates[button];

        // Determine target values based on hover state
        Vector3 targetScale = isHovering ? originalState.originalScale * hoverScale : originalState.originalScale;
        Color targetColor = isHovering ? hoverTintColor : originalState.originalColor;

        // Get current values for smooth transition
        Vector3 startScale = buttonTransform.localScale;
        Color startColor = buttonImage != null ? buttonImage.color : originalState.originalColor;

        float elapsed = 0f;

        // Animate over time
        while (elapsed < hoverAnimationSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hoverAnimationSpeed;

            buttonTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        // Ensure final values are exact
        buttonTransform.localScale = targetScale;
        if (buttonImage != null)
            buttonImage.color = targetColor;
    }

    private void UpdateButtonStates()
    {
        bool hasSave = SaveSystem.HasSaveFile();

        // Disable continue/load buttons if no save exists
        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (loadGameButton != null)
            loadGameButton.interactable = hasSave;
    }

    private void OnNewGame()
    {
        if (!isTransitioning)
            StartCoroutine(NewGameTransition());
    }

    private System.Collections.IEnumerator NewGameTransition()
    {
        PlayClickSound();
        isTransitioning = true;

        yield return new WaitForSeconds(0.2f);

        SaveSystem.DeleteSave();

        // Create new save data with default values
        GameSaveData saveData = new GameSaveData
        {
            currentLevel = SceneManager.GetSceneByBuildIndex(1).name,
            nextLevel = SceneManager.sceneCountInBuildSettings > 2 ?
                SceneManager.GetSceneByBuildIndex(2).name : "",
            playerHealth = 100f,
            playerSanity = 100f,
            playerMagic = 100f,
            saveTime = System.DateTime.Now.ToString(),
            levelsCompleted = 0
        };

        SaveSystem.SaveGame(saveData);
        SceneManager.LoadScene(1);
    }

    private void OnLoadGame()
    {
        LoadSavedGame();
    }

    private void OnContinue()
    {
        LoadSavedGame();
    }

    private void LoadSavedGame()
    {
        if (!isTransitioning)
            StartCoroutine(LoadGameTransition());
    }

    private System.Collections.IEnumerator LoadGameTransition()
    {
        PlayClickSound();
        isTransitioning = true;

        yield return new WaitForSeconds(0.2f);

        GameSaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            // Load the saved level
            SceneManager.LoadScene(data.currentLevel);

            // Restore player stats if available
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.SetHealth(data.playerHealth);
                PlayerStats.Instance.SetSanity(data.playerSanity);
                PlayerStats.Instance.SetMagic(data.playerMagic);
            }
        }
    }

    private void OnSettings()
    {
        PlayClickSound();
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnExit()
    {
        PlayExitSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    private void PlayExitSound()
    {
        if (audioSource != null && exitSound != null)
            audioSource.PlayOneShot(exitSound);
    }
}
