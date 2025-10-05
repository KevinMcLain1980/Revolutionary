using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Status UI (Bottom Left)")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider playerStaminaSlider;
    [SerializeField] private Slider playerMagicSlider;
    [SerializeField] private Text healthText;
    [SerializeField] private Text staminaText;
    [SerializeField] private Text magicText;

    [Header("Inventory Slots")]
    [SerializeField] private Button[] inventorySlots = new Button[7];
    [SerializeField] private Image[] slotCooldownOverlays = new Image[3]; // Only for magic slots (last 3)
    [SerializeField] private Text[] cooldownTexts = new Text[3]; // Cooldown timers for magic slots

    [Header("Boss Health Bar (Top Middle)")]
    [SerializeField] private GameObject bossHealthBarPanel;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private Text bossNameText;
    [SerializeField] private Text bossHealthText;

    [Header("Settings")]
    [SerializeField] private float[] magicSlotCooldowns = { 5f, 8f, 12f }; // Cooldown times for magic slots

    // Player stats
    private float maxHealth = 100f;
    private float currentPlayerHealth = 100f;
    private float maxStamina = 100f;
    private float currentStamina = 100f;
    private float maxMagic = 100f;
    private float currentPlayerMagic = 100f;

    // Boss stats
    private float maxBossHealth = 1000f;
    private float currentBossHealth = 1000f;
    private string bossName = "Ancient Evil";

    // Cooldown tracking
    private float[] magicCooldownTimers = new float[3];
    private bool[] magicSlotsOnCooldown = new bool[3];

    private void Start()
    {
        InitializeUI();
        SetupInventorySlots();
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    // Initialize all UI elements
    private void InitializeUI()
    {
        // Initialize player status UI
        UpdatePlayerHealthUI();
        UpdatePlayerStaminaUI();
        UpdatePlayerMagicUI();

        // Hide boss health bar initially
        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(false);

        // Initialize cooldown overlays
        for (int i = 0; i < slotCooldownOverlays.Length; i++)
        {
            if (slotCooldownOverlays[i] != null)
                slotCooldownOverlays[i].fillAmount = 0f;

            if (cooldownTexts[i] != null)
                cooldownTexts[i].gameObject.SetActive(false);
        }
    }

    // Setup inventory slot click handlers
    private void SetupInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            int slotIndex = i; // Capture index for lambda
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].onClick.AddListener(() => OnInventorySlotClicked(slotIndex));
            }
        }
    }

    // Handle inventory slot clicks
    private void OnInventorySlotClicked(int slotIndex)
    {
        // Regular inventory slots (0-3)
        if (slotIndex < 4)
        {
            UseInventoryItem(slotIndex);
        }
        // Magic spell slots (4-6) with cooldowns
        else
        {
            int magicSlotIndex = slotIndex - 4;
            if (!magicSlotsOnCooldown[magicSlotIndex])
            {
                UseMagicSpell(magicSlotIndex);
                StartMagicCooldown(magicSlotIndex);
            }
        }
    }

    // Use regular inventory item
    private void UseInventoryItem(int slotIndex)
    {
        Debug.Log($"Used inventory item in slot {slotIndex}");
        // Add your item usage logic here
    }

    // Use magic spell
    private void UseMagicSpell(int magicSlotIndex)
    {
        Debug.Log($"Cast magic spell from slot {magicSlotIndex + 4}");

        // Consume magic points
        ModifyPlayerMagic(-20f);

        // Add your spell casting logic here
    }

    // Start cooldown for magic slot
    private void StartMagicCooldown(int magicSlotIndex)
    {
        magicSlotsOnCooldown[magicSlotIndex] = true;
        magicCooldownTimers[magicSlotIndex] = magicSlotCooldowns[magicSlotIndex];

        if (inventorySlots[magicSlotIndex + 4] != null)
            inventorySlots[magicSlotIndex + 4].interactable = false;
    }

    // Update magic spell cooldowns
    private void UpdateCooldowns()
    {
        for (int i = 0; i < magicCooldownTimers.Length; i++)
        {
            if (magicSlotsOnCooldown[i])
            {
                magicCooldownTimers[i] -= Time.deltaTime;

                // Update cooldown overlay
                if (slotCooldownOverlays[i] != null)
                {
                    float fillAmount = magicCooldownTimers[i] / magicSlotCooldowns[i];
                    slotCooldownOverlays[i].fillAmount = fillAmount;
                }

                // Update cooldown text
                if (cooldownTexts[i] != null)
                {
                    cooldownTexts[i].gameObject.SetActive(true);
                    cooldownTexts[i].text = Mathf.Ceil(magicCooldownTimers[i]).ToString();
                }

                // Check if cooldown finished
                if (magicCooldownTimers[i] <= 0f)
                {
                    magicSlotsOnCooldown[i] = false;
                    magicCooldownTimers[i] = 0f;

                    if (inventorySlots[i + 4] != null)
                        inventorySlots[i + 4].interactable = true;

                    if (cooldownTexts[i] != null)
                        cooldownTexts[i].gameObject.SetActive(false);

                    if (slotCooldownOverlays[i] != null)
                        slotCooldownOverlays[i].fillAmount = 0f;
                }
            }
        }
    }

    // Player health management
    public void ModifyPlayerHealth(float amount)
    {
        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth + amount, 0f, maxHealth);
        UpdatePlayerHealthUI();

        // Update GameManager if needed (convert to 0-1 range)
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateHealth(currentPlayerHealth / maxHealth);
    }

    private void UpdatePlayerHealthUI()
    {
        if (playerHealthSlider != null)
            playerHealthSlider.value = currentPlayerHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{(int)currentPlayerHealth}/{(int)maxHealth}";
    }

    // Player stamina management
    public void ModifyPlayerStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        UpdatePlayerStaminaUI();

        // Update GameManager sanity slider for stamina (convert to 0-1 range)
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateSanity(currentStamina / maxStamina);
    }

    private void UpdatePlayerStaminaUI()
    {
        if (playerStaminaSlider != null)
            playerStaminaSlider.value = currentStamina / maxStamina;

        if (staminaText != null)
            staminaText.text = $"{(int)currentStamina}/{(int)maxStamina}";
    }

    // Player magic management
    public void ModifyPlayerMagic(float amount)
    {
        currentPlayerMagic = Mathf.Clamp(currentPlayerMagic + amount, 0f, maxMagic);
        UpdatePlayerMagicUI();

        // Update GameManager magic slider (convert to 0-1 range)
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateMagic(currentPlayerMagic / maxMagic);
    }

    private void UpdatePlayerMagicUI()
    {
        if (playerMagicSlider != null)
            playerMagicSlider.value = currentPlayerMagic / maxMagic;

        if (magicText != null)
            magicText.text = $"{(int)currentPlayerMagic}/{(int)maxMagic}";
    }

    // Boss health bar management
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

        // Hide boss health bar when boss dies
        if (currentBossHealth <= 0f)
        {
            StartCoroutine(HideBossHealthBarAfterDelay(2f));
        }
    }

    private void UpdateBossHealthUI()
    {
        if (bossHealthSlider != null)
            bossHealthSlider.value = currentBossHealth / maxBossHealth;

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

    // Public getters for other scripts
    public float GetCurrentHealth() => currentPlayerHealth;
    public float GetCurrentStamina() => currentStamina;
    public float GetCurrentMagic() => currentPlayerMagic;
    public bool IsMagicSlotOnCooldown(int magicSlotIndex) => magicSlotsOnCooldown[magicSlotIndex];
    public float GetMagicCooldownTime(int magicSlotIndex) => magicCooldownTimers[magicSlotIndex];
}
