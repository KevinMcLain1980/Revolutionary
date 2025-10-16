using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // Singleton instance
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory Slots")]
    [SerializeField] private Item primaryWeapon;
    [SerializeField] private Item secondaryWeapon;
    [SerializeField] private Item itemSlot1;
    [SerializeField] private Item itemSlot2;

    [Header("Magic Spells")]
    [SerializeField] private MagicSpell spell1;
    [SerializeField] private MagicSpell spell2;
    [SerializeField] private MagicSpell spell3;

    [Header("UI References")]
    [SerializeField] private GameObject[] slotSelectedImages; // Slot selection highlight images
    [SerializeField] private GameObject[] itemImages; // General item slot images
    [SerializeField] private GameObject swordImage; // Sword item icon
    [SerializeField] private GameObject healthPotionImage; // Health potion item icon
    [SerializeField] private GameObject keyImage; // Key item icon
    [SerializeField] private TextMeshProUGUI healthPotionCountText; // Counter text for health potions

    private List<Item> inventoryItems = new List<Item>(4);
    private List<MagicSpell> magicSpells = new List<MagicSpell>(3);
    private int selectedSlotIndex = -1; // Currently selected inventory slot
    private int healthPotionCount = 0; // Total number of health potions

    public event Action<List<Item>> OnInventoryChanged;

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize inventory and spell lists
        inventoryItems = new List<Item> { primaryWeapon, secondaryWeapon, itemSlot1, itemSlot2 };
        magicSpells = new List<MagicSpell> { spell1, spell2, spell3 };
    }

    private void Start()
    {
        // Initialize UI for starting items
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            UpdateItemImageForSlot(i);
        }

        // Select first slot by default
        if (inventoryItems.Count > 0)
        {
            SelectInventorySlot(0);
        }
    }

    private void Update()
    {
        // Handle number key input for slot selection
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
        }
    }

    // Add item to inventory at specified slot or first available slot
    public void PickupItem(Item item, int slotIndex = -1)
    {
        // Find empty slot if none specified
        if (slotIndex == -1)
        {
            slotIndex = FindFirstEmptySlot();
            if (slotIndex == -1)
            {
                Debug.LogWarning("Inventory full! Cannot pick up item.");
                return;
            }
        }

        if (slotIndex < 0 || slotIndex > 3) return;

        // Increment health potion counter if picking up a potion
        if (item.itemName.ToLower().Contains("potion") || item.itemName.ToLower().Contains("health"))
        {
            healthPotionCount++;
            UpdateHealthPotionUI();
        }

        inventoryItems[slotIndex] = item;
        UpdateInventorySlot(slotIndex, item);
        UpdateItemImageForSlot(slotIndex);
        OnInventoryChanged?.Invoke(inventoryItems);
        Debug.Log($"Picked up {item.itemName} in slot {slotIndex}");
    }

    // Find first empty item slot (slots 2 and 3)
    private int FindFirstEmptySlot()
    {
        for (int i = 2; i < 4; i++)
        {
            if (inventoryItems[i] == null)
                return i;
        }
        return -1;
    }

    // Add weapon to primary or secondary weapon slot
    public void PickupWeapon(Item weapon, bool isPrimary)
    {
        int slotIndex = isPrimary ? 0 : 1;
        PickupItem(weapon, slotIndex);
    }

    // Add magic spell to specified spell slot
    public void PickupMagicSpell(MagicSpell spell, int spellSlot)
    {
        if (spellSlot < 0 || spellSlot > 2) return;

        magicSpells[spellSlot] = spell;
        Debug.Log($"Learned {spell.spellName} in magic slot {spellSlot}");
    }

    // Use item from specified inventory slot
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return;

        Item item = inventoryItems[slotIndex];
        if (item != null && item.IsUsable())
        {
            // Handle health potion usage
            if (item.itemName.ToLower().Contains("potion") || item.itemName.ToLower().Contains("health"))
            {
                healthPotionCount--;
                if (healthPotionCount < 0) healthPotionCount = 0;
                UpdateHealthPotionUI();

                // Remove potion from slot if count reaches zero
                if (healthPotionCount == 0)
                {
                    inventoryItems[slotIndex] = null;
                    UpdateInventorySlot(slotIndex, null);
                    UpdateItemImageForSlot(slotIndex);
                }
            }
            else
            {
                // Remove non-potion items after use
                inventoryItems[slotIndex] = null;
                UpdateInventorySlot(slotIndex, null);
                UpdateItemImageForSlot(slotIndex);
            }

            item.Use();
            Debug.Log($"Used item: {item.itemName}");
        }
    }

    // Get item from specified slot
    public Item GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return null;
        return inventoryItems[slotIndex];
    }

    // Get magic spell from specified slot
    public MagicSpell GetMagicSpell(int spellSlot)
    {
        if (spellSlot < 0 || spellSlot >= magicSpells.Count) return null;
        return magicSpells[spellSlot];
    }

    // Get primary weapon (slot 0)
    public Item GetPrimaryWeapon() => inventoryItems[0];

    // Get secondary weapon (slot 1)
    public Item GetSecondaryWeapon() => inventoryItems[1];

    // Update serialized slot references
    private void UpdateInventorySlot(int slotIndex, Item item)
    {
        switch (slotIndex)
        {
            case 0: primaryWeapon = item; break;
            case 1: secondaryWeapon = item; break;
            case 2: itemSlot1 = item; break;
            case 3: itemSlot2 = item; break;
        }
    }

    // Select inventory slot and show selection highlight
    public void SelectInventorySlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 4) return;

        selectedSlotIndex = slotIndex;

        // Update slot selection UI
        for (int i = 0; i < slotSelectedImages.Length; i++)
        {
            if (slotSelectedImages[i] != null)
            {
                slotSelectedImages[i].SetActive(i == slotIndex);
            }
        }
    }

    // Update item image visibility and type for specific slot
    private void UpdateItemImageForSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return;

        Item item = inventoryItems[slotIndex];

        // Show or hide item image based on slot contents
        if (itemImages != null && slotIndex < itemImages.Length && itemImages[slotIndex] != null)
        {
            itemImages[slotIndex].SetActive(item != null);
        }

        if (item != null)
        {
            string itemName = item.itemName.ToLower();

            // Show appropriate item icon based on item type
            if (itemName.Contains("sword") && swordImage != null)
            {
                swordImage.SetActive(true);
            }
            else if ((itemName.Contains("potion") || itemName.Contains("health")) && healthPotionImage != null)
            {
                healthPotionImage.SetActive(true);
            }
            else if (itemName.Contains("key") && keyImage != null)
            {
                keyImage.SetActive(true);
            }
        }
        else
        {
            // Hide item image if slot is empty
            if (slotIndex < itemImages.Length && itemImages[slotIndex] != null)
            {
                itemImages[slotIndex].SetActive(false);
            }
        }
    }

    // Update health potion counter text
    private void UpdateHealthPotionUI()
    {
        if (healthPotionCountText != null)
        {
            healthPotionCountText.text = healthPotionCount.ToString();
            healthPotionCountText.gameObject.SetActive(healthPotionCount > 0);
        }
    }
}
