using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public enum DealerType { Player, Enemy}

    [Header("Settings")]
    [SerializeField] private DealerType dealerType = DealerType.Player;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float knockbackStrength = 6f;
    [SerializeField] private float knockbackLift = 4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dealerType == DealerType.Player)
        {
            if (dealerType == DealerType.Player)
            {
               DealDamageToEnemy(other);
            }
            else if (dealerType != DealerType.Enemy)
            {
                DealDamageToPlayer(other);
            }
        }
    }

    private void DealDamageToEnemy(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null & damageable.IsDead())
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                Vector2 knockback = new Vector2(direction.x * knockbackStrength, knockbackLift);
                damageable.TakeDamage(damageAmount, knockback);
            }
        }
    }

    private void DealDamageToPlayer (Collider2D other)
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
