using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory State")]
    [SerializeField] private bool hasSword = true;
    [SerializeField] private int healthPotionCount = 0;
    [SerializeField] private bool hasKey = false;

    [Header("UI References - Slot Selection Overlays")]
    [SerializeField] private GameObject[] slotSelectedImages; // 3 overlays for slots 0-2

    [Header("UI References - Item Images")]
    [SerializeField] private GameObject swordImage;
    [SerializeField] private GameObject healthPotionImage;
    [SerializeField] private GameObject keyImage;
    [SerializeField] private TextMeshProUGUI healthPotionCountText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip potionPickupSound;
    [SerializeField] private AudioClip potionUseSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem healParticleEffect;
    [SerializeField] private bool flashPlayerOnHeal = true;
    [SerializeField] private Color healFlashColor = new Color(0.3f, 1f, 0.3f, 1f); // Light green
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private int flashCount = 2;

    private int selectedSlotIndex = 0;
    private SpriteRenderer playerSpriteRenderer;
    private Coroutine healFlashCoroutine;

    private Door nearbyDoor;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("[PlayerInventory] Singleton instance created");
        }
        else
        {
            // Copy UI references from duplicate to the persistent instance before destroying
            //Debug.Log("[PlayerInventory] Scene reloaded. Transferring UI references from new instance to persistent instance.");

            Instance.slotSelectedImages = this.slotSelectedImages;
            Instance.swordImage = this.swordImage;
            Instance.healthPotionImage = this.healthPotionImage;
            Instance.keyImage = this.keyImage;
            Instance.healthPotionCountText = this.healthPotionCountText;
            Instance.healParticleEffect = this.healParticleEffect;
            Instance.potionPickupSound = this.potionPickupSound;
            Instance.potionUseSound = this.potionUseSound;
            Instance.audioSource = this.audioSource;

            // Refresh component references
            Instance.playerSpriteRenderer = Instance.GetComponent<SpriteRenderer>();
            if (Instance.audioSource == null)
            {
                Instance.audioSource = Instance.GetComponent<AudioSource>();
                if (Instance.audioSource == null)
                {
                    Instance.audioSource = Instance.gameObject.AddComponent<AudioSource>();
                }
            }

            // Update UI to show current inventory state with new references
            Instance.UpdateAllUI();
            Instance.SelectInventorySlot(Instance.selectedSlotIndex);

            Destroy(gameObject);
            return;
        }

        LoadInventory();

        // Get AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Get player sprite renderer for flash effect
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Refind UI references if they're null (happens on scene reload)
        RefreshUIReferences();

        // Ensure slot 0 is selected by default
        selectedSlotIndex = 0;

        UpdateAllUI();
        SelectInventorySlot(0);
        OnInventoryChanged?.Invoke();

        //Debug.Log($"[PlayerInventory] Initialized - Slot 0 selected, Potions: {healthPotionCount}, Sword: {hasSword}, Key: {hasKey}");
    }

    private void RefreshUIReferences()
    {
        if (swordImage == null || healthPotionImage == null || keyImage == null)
        {
            //Debug.LogWarning("[PlayerInventory] UI references lost (scene reload). Attempting to find them...");

            GameObject inventoryUI = GameObject.Find("InventoryUI");
            if (inventoryUI != null)
            {
                if (swordImage == null)
                {
                    Transform found = inventoryUI.transform.Find("SwordImage");
                    if (found != null) swordImage = found.gameObject;
                }

                if (healthPotionImage == null)
                {
                    Transform found = inventoryUI.transform.Find("Healthpotion");
                    if (found != null) healthPotionImage = found.gameObject;
                }

                if (keyImage == null)
                {
                    Transform found = inventoryUI.transform.Find("KeyImage");
                    if (found != null) keyImage = found.gameObject;
                }

                if (healthPotionCountText == null)
                {
                    Transform found = inventoryUI.transform.Find("Healthpotionamount");
                    if (found != null) healthPotionCountText = found.GetComponent<TextMeshProUGUI>();
                }
            }

            if (slotSelectedImages == null || slotSelectedImages.Length == 0 || slotSelectedImages[0] == null)
            {
                if (inventoryUI != null)
                {
                    slotSelectedImages = new GameObject[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Transform found = inventoryUI.transform.Find($"SlotSelected{i}");
                        if (found != null) slotSelectedImages[i] = found.gameObject;
                    }
                }
            }

            //Debug.Log("[PlayerInventory] UI reference refresh complete. Some may still be null if not found.");
        }
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SelectInventorySlot(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SelectInventorySlot(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                SelectInventorySlot(2);
            }

            if (Keyboard.current.leftCtrlKey.wasPressedThisFrame)
            {
                UseSelectedItem();
            }
        }
    }

    public void SelectInventorySlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 3) return;
        if (selectedSlotIndex == slotIndex) return;

        selectedSlotIndex = slotIndex;

        for (int i = 0; i < slotSelectedImages.Length && i < 3; i++)
        {
            if (slotSelectedImages[i] != null)
            {
                slotSelectedImages[i].SetActive(i == slotIndex);
            }
        }

        //Debug.Log($"[PlayerInventory] Selected slot {slotIndex}");
        SaveInventory();
    }

    private void UseSelectedItem()
    {
        switch (selectedSlotIndex)
        {
            case 0:
                UseSword();
                break;
            case 1:
                UseHealthPotion();
                break;
            case 2:
                UseKey();
                break;
        }
    }

    private void UseSword()
    {
        if (!hasSword) return;

        var playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.PerformAttack();
        }
        else
        {
            //Debug.LogWarning("PlayerCombat component not found.");
        }
    }

    private void UseHealthPotion()
    {
        if (healthPotionCount <= 0)
        {
            //Debug.Log("[PlayerInventory] Cannot use potion - count is 0");
            return;
        }

        healthPotionCount--;
        healthPotionCount = Mathf.Max(0, healthPotionCount);

        // Play use sound
        if (potionUseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(potionUseSound, sfxVolume);
        }

        // Play particle effect
        if (healParticleEffect != null)
        {
            healParticleEffect.Play();
        }

        // Flash player sprite
        if (flashPlayerOnHeal && playerSpriteRenderer != null)
        {
            if (healFlashCoroutine != null)
            {
                StopCoroutine(healFlashCoroutine);
            }
            healFlashCoroutine = StartCoroutine(HealFlashEffect());
        }

        var playerStats = PlayerStats.Instance;
        if (playerStats != null)
        {
            playerStats.Heal(20);
            //Debug.Log($"[PlayerInventory] Used potion, healed 20 HP. Remaining: {healthPotionCount}");
        }
        else
        {
            //Debug.LogWarning("[PlayerInventory] PlayerStats instance not found.");
        }

        UpdateHealthPotionUI();
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    private void UseKey()
    {
        if (!hasKey)
        {
            //Debug.Log("Find the key!");
            return;
        }

        if (nearbyDoor != null)
        {
            nearbyDoor.OpenChest(this);
        }
        else
        {
            //Debug.Log("No chest nearby.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            nearbyDoor = collision.GetComponent<Door>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        nearbyDoor = null;
    }
    private System.Collections.IEnumerator HealFlashEffect()
    {
        Color originalColor = playerSpriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            // Flash to heal color
            playerSpriteRenderer.color = healFlashColor;
            yield return new WaitForSeconds(flashDuration / 2f);

            // Flash back to original
            playerSpriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration / 2f);
        }

        // Ensure we end on original color
        playerSpriteRenderer.color = originalColor;
        healFlashCoroutine = null;
    }

    public void PickupHealthPotion(int amount = 1)
    {
        if (amount <= 0) return;

        healthPotionCount += amount;
        healthPotionCount = Mathf.Max(0, healthPotionCount);

        // Play pickup sound
        if (potionPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(potionPickupSound, sfxVolume);
        }

        //Debug.Log($"[PlayerInventory] Picked up {amount} potion(s). Total now: {healthPotionCount}");

        UpdateHealthPotionUI();
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    public void PickupKey()
    {
        hasKey = true;
        UpdateKeyUI();
        OnInventoryChanged?.Invoke();
        SaveInventory();

        //Debug.Log("Picked up key.");
    }

    public void RemoveKey()
    {
        hasKey = false;
        UpdateKeyUI();
        OnInventoryChanged?.Invoke();
        SaveInventory();

        //Debug.Log("Key removed/used.");
    }

    private void UpdateAllUI()
    {
        UpdateSwordUI();
        UpdateHealthPotionUI();
        UpdateKeyUI();
    }

    private void UpdateSwordUI()
    {
        if (swordImage != null)
        {
            swordImage.SetActive(hasSword);
        }
    }

    private void UpdateHealthPotionUI()
    {
        bool shouldShowPotion = healthPotionCount > 0;

        if (healthPotionImage != null)
        {
            healthPotionImage.SetActive(shouldShowPotion);
            //Debug.Log($"[PlayerInventory] Health potion image set to: {shouldShowPotion}");
        }

        if (healthPotionCountText != null)
        {
            healthPotionCountText.text = healthPotionCount.ToString();
            healthPotionCountText.gameObject.SetActive(shouldShowPotion);
            //Debug.Log($"[PlayerInventory] Health potion counter text set to: {healthPotionCount}, visible: {shouldShowPotion}");
        }
    }

    private void UpdateKeyUI()
    {
        if (keyImage != null)
        {
            keyImage.SetActive(hasKey);
        }
    }

    public bool HasSword() => hasSword;
    public int GetHealthPotionCount() => healthPotionCount;
    public bool HasKey() => hasKey;
    public int GetSelectedSlotIndex() => selectedSlotIndex;

    public Item GetPrimaryWeapon()
    {
        return null;
    }

    [Serializable]
    private class InventoryData
    {
        public bool hasSword = true;
        public int healthPotionCount = 0;
        public bool hasKey = false;
        public int selectedSlotIndex = 0;
        public int version = 1;
    }

    private void SaveInventory()
    {
        InventoryData data = new InventoryData
        {
            hasSword = this.hasSword,
            healthPotionCount = this.healthPotionCount,
            hasKey = this.hasKey,
            selectedSlotIndex = this.selectedSlotIndex,
            version = 1
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerInventory", json);
        PlayerPrefs.Save();

    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("PlayerInventory"))
        {
            string json = PlayerPrefs.GetString("PlayerInventory");
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);

            if (data.version == 1)
            {
                this.hasSword = data.hasSword;
                this.healthPotionCount = data.healthPotionCount;
                this.hasKey = data.hasKey;
                this.selectedSlotIndex = data.selectedSlotIndex;

                //////Debug.Log("Inventory loaded successfully.");
            }
            else
            {
                ////Debug.LogWarning("Unknown save version. Using defaults.");
            }
        }
        else
        {
            ////Debug.Log("No save file found. Using default inventory.");
        }
    }

    public void ResetInventory()
    {
        hasSword = true;
        healthPotionCount = 0;
        hasKey = false;
        selectedSlotIndex = 0;

        UpdateAllUI();
        SelectInventorySlot(0);
        OnInventoryChanged?.Invoke();
        SaveInventory();


    }

}
