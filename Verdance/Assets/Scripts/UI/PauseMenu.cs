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

    private bool isPaused = false; // Tracks if the game is currently paused

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
        // Toggle pause menu when ESC is pressed
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC pressed");
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
            Debug.Log("Setting up Resume button");
            resumeButton.onClick.AddListener(Resume);
            AddHoverSound(resumeButton);
        }
        else
        {
            Debug.LogError("Resume button is null!");
        }

        if (restartButton != null)
        {
            Debug.Log("Setting up Restart button");
            restartButton.onClick.AddListener(Restart);
            AddHoverSound(restartButton);
        }
        else
        {
            Debug.LogError("Restart button is null!");
        }

        if (settingsButton != null)
        {
            Debug.Log("Setting up Settings button");
            settingsButton.onClick.AddListener(ToggleSettings);
            AddHoverSound(settingsButton);
        }
        else
        {
            Debug.LogError("Settings button is null!");
        }

        if (exitButton != null)
        {
            Debug.Log("Setting up Exit button");
            exitButton.onClick.AddListener(ExitToMainMenu);
            AddHoverSound(exitButton);
        }
        else
        {
            Debug.LogError("Exit button is null!");
        }
    }

    // Add hover sound effect to a button using EventTrigger
    private void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    // Pause the game and show the pause menu
    public void Pause()
    {
        Debug.Log("Pause() called");
        isPaused = true;
        if (pauseMenuPanel != null)
        {
            Debug.Log("Setting pause panel active");
            pauseMenuPanel.SetActive(true);
            Debug.Log("Pause panel active state: " + pauseMenuPanel.activeSelf);
        }
        else
        {
            Debug.LogError("pauseMenuPanel is null!");
        }
        Time.timeScale = 0f; // Freeze game time
    }

    // Resume the game and hide the pause menu
    public void Resume()
    {
        Debug.Log("Resume button clicked");
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    // Restart the current level
    private void Restart()
    {
        Debug.Log("Restart button clicked");
        PlayClickSound();
        Time.timeScale = 1f; // Reset time scale before loading scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ToggleSettings()
    {
        Debug.Log("Settings button clicked");
        PlayClickSound();

        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
            Debug.Log($"Settings panel now {(!isActive ? "active" : "inactive")}");

            if (pauseMenuPanel != null && !isActive)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Settings panel not assigned!");
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
        Debug.Log("Exit button clicked");
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
