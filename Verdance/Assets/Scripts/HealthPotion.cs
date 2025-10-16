using UnityEngine;

[CreateAssetMenu(fileName = "Health Potion", menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    [Header("Potion Settings")]
    public int healAmount = 10;

    public override void Use()
    {
        PlayerStats health = FindFirstObjectByType<PlayerStats>();

        if (health != null)
        {
            health.Heal(healAmount);
            Debug.Log($"{itemName} healed {healAmount}");
        }

    }
}
