using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Status UI (Bottom Left)")]
    [SerializeField] private Image playerHealthSlider;
    [SerializeField] private Image playerSanitySlider;
    [SerializeField] private Image playerMagicSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text sanityText;
    [SerializeField] private TMP_Text magicText;

    [Header("Inventory Slots")]
    [SerializeField] private Button[] inventorySlots = new Button[3];

    [Header("Boss Health Bar (Top Middle)")]
    [SerializeField] private GameObject bossHealthBarPanel;
    [SerializeField] private Image bossHealthSlider;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossHealthText;

    [Header("Lives System")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject[] lifeIcons = new GameObject[3];

    [Header("GameOver")]
    [SerializeField] private GameObject gameeOverScreen;
    [SerializeField] private AudioSource levelMusicSource;
    [SerializeField] private AudioSource gameOverMusicSource;

    [Header("Message Popup")]
    [SerializeField] private GameObject messagePopup;
    [SerializeField] private TMP_Text messageText;

    private PlayerStats playerStats;
    private PlayerInventory playerInventory;
    private PlayerController2D playerController;
    private PlayerRespawn playerRespawn;

    private float maxBossHealth = 1000f;
    private float currentBossHealth = 1000f;
    private string bossName = "Ancient Evil";

    private void Start()
    {
        playerStats = PlayerStats.Instance;
        playerInventory = PlayerInventory.Instance;
        playerController = FindFirstObjectByType<PlayerController2D>();
        playerRespawn = FindFirstObjectByType<PlayerRespawn>();

        if (playerStats != null)
        {
            playerStats.OnHealthChanged += UpdatePlayerHealthUI;
            playerStats.OnMagicChanged += UpdatePlayerMagicUI;
            playerStats.OnSanityChanged += UpdatePlayerSanityUI;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateInventoryUI;
        }

        if (playerRespawn != null)
        {
            playerRespawn.OnLivesChanged += UpdateLivesUI;
            playerRespawn.OnGameOver += ShowGameOver;
            UpdateLivesUI(playerRespawn.GetCurrentLives());
        }

        InitializeUI();
        SetupInventorySlots();
    }



    private void InitializeUI()
    {
        if (playerStats != null)
        {
            UpdatePlayerHealthUI(playerStats.GetCurrentHealth(), playerStats.GetMaxHealth());
            UpdatePlayerSanityUI(playerStats.GetCurrentSanity(), playerStats.GetMaxSanity());
            UpdatePlayerMagicUI(playerStats.GetCurrentMagic(), playerStats.GetMaxMagic());
        }

        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(false);

        if (gameeOverScreen != null)
        {
            gameeOverScreen.SetActive(false);
        }

        if (messagePopup != null)
        {
            messagePopup.SetActive(false); 
        }
    }

    public void ShowMessage(string message, float duration = 3f)
    {
        if (message != null && messageText != null)
        {
            messageText.text = message;
            messagePopup.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(duration));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messagePopup != null)
        {
            messagePopup.SetActive(false);
        }
    }

    private void SetupInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            int slotIndex = i;
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].onClick.AddListener(() => OnInventorySlotClicked(slotIndex));
            }
        }
    }

    private void OnInventorySlotClicked(int slotIndex)
    {
        if (playerInventory != null && slotIndex >= 0 && slotIndex < 3)
        {
            playerInventory.SelectInventorySlot(slotIndex);
        }
    }

    private void UpdatePlayerHealthUI(float current, float max)
    {
        if (playerHealthSlider != null)
            playerHealthSlider.fillAmount = current / max;

        if (healthText != null)
            healthText.text = $"{(int)current}/{(int)max}";

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealth(current / max);
    }

    private void UpdatePlayerSanityUI(float current, float max)
    {
        if (playerSanitySlider != null)
            playerSanitySlider.fillAmount = current / max;

        if (sanityText != null)
            sanityText.text = $"{(int)current}/{(int)max}";

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateSanity(current / max);
    }

    private void UpdatePlayerMagicUI(float current, float max)
    {
        if (playerMagicSlider != null)
            playerMagicSlider.fillAmount = current / max;

        if (magicText != null)
            magicText.text = $"{(int)current}/{(int)max}";

        if (GameManager.Instance != null)
            GameManager.Instance.UpdateMagic(current / max);
    }

    private void UpdateInventoryUI()
    {
    }

    public void ShowBossHealthBar(string name, float maxHealth)
    {
        bossName = name;
        maxBossHealth = maxHealth;
        currentBossHealth = maxHealth;

        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(true);

        UpdateBossHealthUI();
    }

    public void HideBossHealthBar()
    {
        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(false);
    }

    public void UpdateBossHealth(float newHealth)
    {
        currentBossHealth = Mathf.Clamp(newHealth, 0f, maxBossHealth);
        UpdateBossHealthUI();

        if (currentBossHealth <= 0f)
        {
            StartCoroutine(HideBossHealthBarAfterDelay(2f));
        }
    }

    private void UpdateBossHealthUI()
    {
        if (bossHealthSlider != null)
            bossHealthSlider.fillAmount = currentBossHealth / maxBossHealth;

        if (bossNameText != null)
            bossNameText.text = bossName;

        if (bossHealthText != null)
            bossHealthText.text = $"{(int)currentBossHealth}/{(int)maxBossHealth}";
    }

    private IEnumerator HideBossHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideBossHealthBar();
    }

    private void UpdateLivesUI(int Lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {Lives}";
        }

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if(lifeIcons[i] != null)
            {
                lifeIcons[i].SetActive(i < Lives);
            }
        }
    }

    private void ShowGameOver()
    {
        if (gameeOverScreen != null)
        {
            gameeOverScreen.SetActive(true);
        }

        Time.timeScale = 0f;

        if (levelMusicSource != null)
        {
            levelMusicSource.Stop();
            levelMusicSource.enabled = false;
        }

        if (gameOverMusicSource != null)
        {
            gameOverMusicSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdatePlayerHealthUI;
            playerStats.OnMagicChanged -= UpdatePlayerMagicUI;
            playerStats.OnSanityChanged -= UpdatePlayerSanityUI;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateInventoryUI;
        }

        if (playerRespawn != null)
        {
            playerRespawn.OnLivesChanged -= UpdateLivesUI;
            playerRespawn.OnGameOver -= ShowGameOver;
        }
    }
}