using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
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

    private List<Item> inventoryItems = new List<Item>(4);
    private List<MagicSpell> magicSpells = new List<MagicSpell>(3);

    public event Action<List<Item>> OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        inventoryItems = new List<Item> { primaryWeapon, secondaryWeapon, itemSlot1, itemSlot2 };
        magicSpells = new List<MagicSpell> { spell1, spell2, spell3 };
    }

    public void PickupItem(Item item, int slotIndex = -1)
    {
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

        inventoryItems[slotIndex] = item;
        UpdateInventorySlot(slotIndex, item);
        OnInventoryChanged?.Invoke(inventoryItems);
        Debug.Log($"Picked up {item.itemName} in slot {slotIndex}");
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 2; i < 4; i++)
        {
            if (inventoryItems[i] == null)
                return i;
        }
        return -1;
    }

    public void PickupWeapon(Item weapon, bool isPrimary)
    {
        int slotIndex = isPrimary ? 0 : 1;
        PickupItem(weapon, slotIndex);
    }

    public void PickupMagicSpell(MagicSpell spell, int spellSlot)
    {
        if (spellSlot < 0 || spellSlot > 2) return;

        magicSpells[spellSlot] = spell;
        Debug.Log($"Learned {spell.spellName} in magic slot {spellSlot}");
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return;

        Item item = inventoryItems[slotIndex];
        if (item != null && item.IsUsable())
        {
            item.Use();
            Debug.Log($"Used item: {item.itemName}");
        }
    }

    public Item GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count) return null;
        return inventoryItems[slotIndex];
    }

    public MagicSpell GetMagicSpell(int spellSlot)
    {
        if (spellSlot < 0 || spellSlot >= magicSpells.Count) return null;
        return magicSpells[spellSlot];
    }

    public Item GetPrimaryWeapon() => inventoryItems[0];
    public Item GetSecondaryWeapon() => inventoryItems[1];

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
}
