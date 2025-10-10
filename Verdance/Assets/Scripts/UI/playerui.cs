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
    [SerializeField] private Button[] inventorySlots = new Button[7];
    [SerializeField] private Image[] slotCooldownOverlays = new Image[3];
    [SerializeField] private TMP_Text[] cooldownTexts = new TMP_Text[3];

    [Header("Boss Health Bar (Top Middle)")]
    [SerializeField] private GameObject bossHealthBarPanel;
    [SerializeField] private Image bossHealthSlider;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text bossHealthText;

    [Header("Settings")]
    [SerializeField] private float[] magicSlotCooldowns = { 5f, 8f, 12f };

    private PlayerStats playerStats;
    private PlayerInventory playerInventory;
    private PlayerController2D playerController;

    private float maxBossHealth = 1000f;
    private float currentBossHealth = 1000f;
    private string bossName = "Ancient Evil";

    private float[] magicCooldownTimers = new float[3];
    private bool[] magicSlotsOnCooldown = new bool[3];

    private void Start()
    {
        playerStats = PlayerStats.Instance;
        playerInventory = PlayerInventory.Instance;
        playerController = FindFirstObjectByType<PlayerController2D>();

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

        InitializeUI();
        SetupInventorySlots();
    }

    private void Update()
    {
        UpdateCooldowns();
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) OnInventorySlotClicked(0);
        if (keyboard.digit2Key.wasPressedThisFrame) OnInventorySlotClicked(1);
        if (keyboard.digit3Key.wasPressedThisFrame) OnInventorySlotClicked(2);
        if (keyboard.digit4Key.wasPressedThisFrame) OnInventorySlotClicked(3);
        if (keyboard.digit5Key.wasPressedThisFrame) OnInventorySlotClicked(4);
        if (keyboard.digit6Key.wasPressedThisFrame) OnInventorySlotClicked(5);
        if (keyboard.digit7Key.wasPressedThisFrame) OnInventorySlotClicked(6);
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

        for (int i = 0; i < slotCooldownOverlays.Length; i++)
        {
            if (slotCooldownOverlays[i] != null)
                slotCooldownOverlays[i].fillAmount = 0f;

            if (cooldownTexts[i] != null)
                cooldownTexts[i].gameObject.SetActive(false);
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
        if (slotIndex < 4)
        {
            UseInventoryItem(slotIndex);
        }
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

    private void UseInventoryItem(int slotIndex)
    {
        if (playerInventory != null)
        {
            playerInventory.UseItem(slotIndex);
        }
    }

    private void UseMagicSpell(int magicSlotIndex)
    {
        if (playerInventory == null) return;

        MagicSpell spell = playerInventory.GetMagicSpell(magicSlotIndex);
        if (spell != null && playerStats != null)
        {
            if (playerStats.ConsumeMagic(spell.manaCost))
            {
                Vector3 playerPos = playerController != null ? playerController.transform.position : Vector3.zero;
                Vector3 direction = playerController != null ? new Vector3(playerController.transform.localScale.x, 0, 0) : Vector3.right;
                spell.Cast(playerPos, direction);
            }
            else
            {
                Debug.Log("Not enough magic!");
            }
        }
    }

    private void StartMagicCooldown(int magicSlotIndex)
    {
        magicSlotsOnCooldown[magicSlotIndex] = true;
        magicCooldownTimers[magicSlotIndex] = magicSlotCooldowns[magicSlotIndex];

        if (inventorySlots[magicSlotIndex + 4] != null)
            inventorySlots[magicSlotIndex + 4].interactable = false;
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < magicCooldownTimers.Length; i++)
        {
            if (magicSlotsOnCooldown[i])
            {
                magicCooldownTimers[i] -= Time.deltaTime;

                if (slotCooldownOverlays[i] != null)
                {
                    float fillAmount = magicCooldownTimers[i] / magicSlotCooldowns[i];
                    slotCooldownOverlays[i].fillAmount = fillAmount;
                }

                if (cooldownTexts[i] != null)
                {
                    cooldownTexts[i].gameObject.SetActive(true);
                    cooldownTexts[i].text = Mathf.Ceil(magicCooldownTimers[i]).ToString();
                }

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

    private void UpdateInventoryUI(System.Collections.Generic.List<Item> items)
    {
        for (int i = 0; i < 4; i++)
        {
            if (inventorySlots[i] != null)
            {
                var slotImage = inventorySlots[i].GetComponent<UnityEngine.UI.Image>();
                if (i < items.Count && items[i] != null)
                {
                    if (slotImage != null && items[i].itemIcon != null)
                    {
                        slotImage.sprite = items[i].itemIcon;
                        slotImage.enabled = true;
                    }
                }
                else
                {
                    if (slotImage != null)
                        slotImage.enabled = false;
                }
            }
        }
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
    }
}
