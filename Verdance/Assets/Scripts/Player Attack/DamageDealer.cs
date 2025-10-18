using UnityEngine;
using System.Collections.Generic;

public class DamageDealer : MonoBehaviour
{
    public enum DealerType { Player, Enemy }

    [Header("Settings")]
    [SerializeField] private DealerType dealerType = DealerType.Player;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float knockbackStrength = 6f;
    [SerializeField] private float knockbackLift = 4f;
    [SerializeField] private LayerMask targetLayers;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [Range(0f, 1f)][SerializeField] private float volume = 0.7f;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    private void OnEnable()
    {
        hitEnemies.Clear();
    }

    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"DamageDealer hit: {other.gameObject.name}, Tag: {other.tag}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}, DealerType: {dealerType}");

        if (targetLayers != 0 && ((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            //Debug.Log($"Ignoring {other.name} - not in target layers");
            return;
        }

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
        GameObject enemyRoot = other.transform.root.gameObject;

        if (hitEnemies.Contains(enemyRoot))
        {
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null)
        {
            damageable = other.GetComponent<IDamageable>();
        }

        if (damageable == null || damageable.IsDead())
        {
            return;
        }

        if (!other.transform.root.CompareTag("Enemy"))
        {
            return;
        }

        hitEnemies.Add(enemyRoot);

        Vector2 targetCenter = other.bounds.center;
        Vector2 direction = (targetCenter - (Vector2)transform.position).normalized;
        Vector2 knockback = new Vector2(direction.x * knockbackStrength, knockbackLift);

        //Debug.Log($"Dealing {damageAmount} damage to {other.transform.root.name}");

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, other.transform.position, volume);
        }

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
