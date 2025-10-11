using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public enum DealerType { Player, Enemy }

    [Header("Settings")]
    [SerializeField] private DealerType dealerType = DealerType.Player;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float knockbackStrength = 6f;
    [SerializeField] private float knockbackLift = 4f;

    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"DamageDealer hit: {other.gameObject.name}, Tag: {other.tag}, DealerType: {dealerType}");

        if (dealerType == DealerType.Player)
        {
            DealDamageToEnemy(other);
        }
        else if (dealerType == DealerType.Enemy)
        {
            DealDamageToPlayer(other);
        }
    }

    private void DealDamageToEnemy(Collider2D other)
    {
        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = other.GetComponent<IDamageable>();
        }

        if (damageable == null || damageable.IsDead())
        {
            Debug.Log($"No IDamageable found on {other.name} or already dead");
            return;
        }

        if (!other.transform.root.CompareTag("Enemy"))
        {
            Debug.Log($"{other.transform.root.name} is not tagged as Enemy");
            return;
        }

        Vector2 targetCenter = other.bounds.center;
        Vector2 direction = (targetCenter - (Vector2)transform.position).normalized;
        Vector2 knockback = new Vector2(direction.x * knockbackStrength, knockbackLift);

        Debug.Log($"Dealing {damageAmount} damage to {other.transform.root.name}");
        damageable.TakeDamage(damageAmount, knockback);
    }

    private void DealDamageToPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = PlayerStats.Instance;
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageAmount);
            }

            PlayerController2D playerController = other.GetComponent<PlayerController2D>();
            if (playerController != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                Vector2 knockback = new Vector2(direction.x * knockbackStrength, knockbackLift);
                playerController.TakeDamage((int)damageAmount, knockback);
            }
        }
    }
}
