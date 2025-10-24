using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Manages the pause menu functionality, including pausing the game and handling menu buttons
public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [SerializeField] private GameObject pauseMenuPanel; // Main pause menu panel
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private SettingsMenu settingsMenu;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip hoverSound; // Sound when hovering over buttons
    [SerializeField] private AudioClip clickSound; // Sound when clicking buttons
    [SerializeField] private AudioSource audioSource;

    [Header("Hover Visual Feedback")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverAnimationSpeed = 0.15f;
    [SerializeField] private Color hoverTintColor = new Color(1f, 1f, 0.7f, 1f);

    private bool isPaused = false; // Tracks if the game is currently paused
    public static bool IsGamePaused { get; private set; } = false;
    private System.Collections.Generic.Dictionary<Button, ButtonState> buttonOriginalStates = new System.Collections.Generic.Dictionary<Button, ButtonState>();
    private System.Collections.Generic.Dictionary<Button, Coroutine> activeButtonCoroutines = new System.Collections.Generic.Dictionary<Button, Coroutine>();

    private struct ButtonState
    {
        public Color originalColor;
        public Vector3 originalScale;
    }

    private void Start()
    {
        SetupButtons();

        // Hide pause menu and settings panel at start
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        // Toggle pause menu when ESC is pressed
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //Debug.Log("ESC pressed");
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // Initialize button click listeners and hover sounds
    private void SetupButtons()
    {
        if (resumeButton != null)
        {
            //Debug.Log("Setting up Resume button");
            resumeButton.onClick.AddListener(Resume);
            AddHoverSound(resumeButton);
        }
        else
        {
            //Debug.LogError("Resume button is null!");
        }

        if (restartButton != null)
        {
            //Debug.Log("Setting up Restart button");
            restartButton.onClick.AddListener(Restart);
            AddHoverSound(restartButton);
        }
        else
        {
            //Debug.LogError("Restart button is null!");
        }

        if (settingsButton != null)
        {
            //Debug.Log("Setting up Settings button");
            settingsButton.onClick.AddListener(ToggleSettings);
            AddHoverSound(settingsButton);
        }
        else
        {
            //Debug.LogError("Settings button is null!");
        }

        if (exitButton != null)
        {
            //Debug.Log("Setting up Exit button");
            exitButton.onClick.AddListener(ExitToMainMenu);
            AddHoverSound(exitButton);
        }
        else
        {
            //Debug.LogError("Exit button is null!");
        }
    }

    // Add hover sound effect to a button using EventTrigger
    private void AddHoverSound(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = new ButtonState
        {
            originalColor = buttonImage != null ? buttonImage.color : Color.white,
            originalScale = button.transform.localScale
        };
        buttonOriginalStates[button] = originalState;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { OnButtonHoverEnter(button); });
        trigger.triggers.Add(hoverEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnButtonHoverExit(button); });
        trigger.triggers.Add(exitEntry);
    }

    private void OnButtonHoverEnter(Button button)
    {
        if (!button.interactable || !gameObject.activeInHierarchy) return;

        PlayHoverSound();

        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, true));
    }

    private void OnButtonHoverExit(Button button)
    {
        if (!button.interactable || !gameObject.activeInHierarchy) return;

        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, false));
    }

    private System.Collections.IEnumerator AnimateButtonHover(Button button, bool isHovering)
    {
        if (!buttonOriginalStates.ContainsKey(button)) yield break;

        Transform buttonTransform = button.transform;
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = buttonOriginalStates[button];

        Vector3 targetScale = isHovering ? originalState.originalScale * hoverScale : originalState.originalScale;
        Color targetColor = isHovering ? hoverTintColor : originalState.originalColor;

        Vector3 startScale = buttonTransform.localScale;
        Color startColor = buttonImage != null ? buttonImage.color : originalState.originalColor;

        float elapsed = 0f;

        while (elapsed < hoverAnimationSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / hoverAnimationSpeed;

            buttonTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        buttonTransform.localScale = targetScale;
        if (buttonImage != null)
            buttonImage.color = targetColor;
    }

    // Pause the game and show the pause menu
    public void Pause()
    {
        //Debug.Log("Pause() called");
        isPaused = true;
            IsGamePaused = true;
            if (pauseMenuPanel != null)
            {
                //Debug.Log("Setting pause panel active");
                pauseMenuPanel.SetActive(true);
                //Debug.Log("Pause panel active state: " + pauseMenuPanel.activeSelf);
            }
            else
            {
                //Debug.LogError("pauseMenuPanel is null!");
            }
            Time.timeScale = 0f; // Freeze game time
    }

    // Resume the game and hide the pause menu
    public void Resume()
    {
        //Debug.Log("Resume button clicked");
        isPaused = false;
            IsGamePaused = false;
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game time
    }

    // Restart the current level
    private void Restart()
    {
        //Debug.Log("Restart button clicked");
        PlayClickSound();
        Time.timeScale = 1f; // Reset time scale before loading scene

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            GameSaveData saveData = SaveSystem.LoadGame();
            int initialPotions = saveData != null ? saveData.initialHealthPotionCount : 0;
            inventory.ResetForRestart(initialPotions);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ToggleSettings()
    {
        //Debug.Log("Settings button clicked");
        PlayClickSound();

        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            //Debug.Log($"Settings panel now {(!isActive ? "active" : "inactive")}");
        }
        else
        {
            //Debug.LogError("Settings panel not assigned!");
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    // Exit to the main menu (scene 0)
    private void ExitToMainMenu()
    {
        //Debug.Log("Exit button clicked");
        PlayClickSound();
        Time.timeScale = 1f; // Reset time scale before loading scene
        SceneManager.LoadScene(0);
    }

    // Play the hover sound effect
    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    // Play the click sound effect
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}