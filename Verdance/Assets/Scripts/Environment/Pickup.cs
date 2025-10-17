using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, Magic, Sanity, HealthPotion, Key }

    [Header("Pickup Settings")]
    public PickupType type = PickupType.Health;
    public float amount = 25f;
    public bool consumeIfFull = false;

    [Header("Potion Settings")]
    public int potionAmount = 1;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError($"[Pickup] {gameObject.name} has no Collider2D!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check both the colliding object and its root for Player tag
        bool isPlayer = other.CompareTag("Player") || other.transform.root.CompareTag("Player");

        if (!isPlayer) return;

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

            case PickupType.HealthPotion:
                if (inventory != null)
                {
                    inventory.PickupHealthPotion(potionAmount);
                    consumed = true;
                }
                break;

            case PickupType.Key:
                if (inventory != null)
                {
                    inventory.PickupKey();
                    consumed = true;
                }
                break;
        }

        if (consumed)
            Destroy(gameObject);
    }
}
