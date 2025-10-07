using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Settings Panel (Optional)")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip exitSound;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        SetupButtons();
        UpdateButtonStates();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

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
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }

    private void UpdateButtonStates()
    {
        bool hasSave = SaveSystem.HasSaveFile();

        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (loadGameButton != null)
            loadGameButton.interactable = hasSave;
    }

    private void OnNewGame()
    {
        SaveSystem.DeleteSave();
        SaveSystem.SaveGame(3);
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
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            SceneManager.LoadScene(data.currentLevel);
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
