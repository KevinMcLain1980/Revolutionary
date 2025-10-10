using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public bool consumable = false;

    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
    }

    public virtual bool IsUsable()
    {
        return true;
    }
}

public enum ItemType
{
    Weapon,
    Consumable,
    QuestItem,
    Key
}
