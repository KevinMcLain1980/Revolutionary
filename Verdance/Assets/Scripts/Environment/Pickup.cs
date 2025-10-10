using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, Magic, Sanity, Item, MagicSpell, PrimaryWeapon, SecondaryWeapon }

    [Header("Pickup Settings")]
    public PickupType type = PickupType.Health;
    public float amount = 25f;
    public bool consumeIfFull = false;

    [Header("Item/Weapon/Spell")]
    public Item itemData;
    public MagicSpell spellData;
    public int targetSlot = 0;

    private void OnTriggerEnter(Collider other)
    {
        var playerRoot = other.transform.root;

        if (!playerRoot.CompareTag("Player")) return;

        bool consumed = false;
        PlayerStats stats = PlayerStats.Instance;
        PlayerInventory inventory = PlayerInventory.Instance;

        switch (type)
        {
            case PickupType.Health:
                if (stats != null)
                {
                    float before = stats.GetCurrentHealth();
                    stats.Heal(amount);
                    consumed = (stats.GetCurrentHealth() > before || consumeIfFull);
                }
                break;

            case PickupType.Magic:
                if (stats != null)
                {
                    float before = stats.GetCurrentMagic();
                    stats.SetMagic(stats.GetCurrentMagic() + amount);
                    consumed = (stats.GetCurrentMagic() > before || consumeIfFull);
                }
                break;

            case PickupType.Sanity:
                if (stats != null)
                {
                    float before = stats.GetCurrentSanity();
                    stats.RestoreSanity(amount);
                    consumed = (stats.GetCurrentSanity() > before || consumeIfFull);
                }
                break;

            case PickupType.Item:
                if (inventory != null && itemData != null)
                {
                    inventory.PickupItem(itemData, targetSlot);
                    consumed = true;
                }
                break;

            case PickupType.PrimaryWeapon:
                if (inventory != null && itemData != null)
                {
                    inventory.PickupWeapon(itemData, true);
                    consumed = true;
                }
                break;

            case PickupType.SecondaryWeapon:
                if (inventory != null && itemData != null)
                {
                    inventory.PickupWeapon(itemData, false);
                    consumed = true;
                }
                break;

            case PickupType.MagicSpell:
                if (inventory != null && spellData != null)
                {
                    inventory.PickupMagicSpell(spellData, targetSlot);
                    consumed = true;
                }
                break;
        }

        if (consumed)
            Destroy(gameObject);
    }
}
