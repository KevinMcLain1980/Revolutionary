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

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private SettingsMenu settingsMenu;

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
        GameSaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            SceneManager.LoadScene(data.currentLevel);

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
