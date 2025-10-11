using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    [Header("Consumable Effects")]
    public float healthRestore = 0f;
    public float sanityRestore = 0f;
    public float magicRestore = 0f;
    public bool singleUse = true;

    public override void Use()
    {
        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return;

        if (healthRestore > 0)
        {
            stats.Heal(healthRestore);
            Debug.Log($"Restored {healthRestore} health");
        }

        if (sanityRestore > 0)
        {
            stats.RestoreSanity(sanityRestore);
            Debug.Log($"Restored {sanityRestore} sanity");
        }

        if (magicRestore > 0)
        {
            stats.SetMagic(stats.GetCurrentMagic() + magicRestore);
            Debug.Log($"Restored {magicRestore} magic");
        }

        if (singleUse)
        {
            Debug.Log($"{itemName} consumed");
        }
    }

    public override bool IsUsable()
    {
        PlayerStats stats = PlayerStats.Instance;
        if (stats == null) return false;

        if (healthRestore > 0 && stats.GetCurrentHealth() >= stats.GetMaxHealth()) return false;
        if (sanityRestore > 0 && stats.GetCurrentSanity() >= stats.GetMaxSanity()) return false;
        if (magicRestore > 0 && stats.GetCurrentMagic() >= stats.GetMaxMagic()) return false;

        return true;
    }
}
