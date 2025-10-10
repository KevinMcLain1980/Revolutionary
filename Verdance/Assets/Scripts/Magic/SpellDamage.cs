using UnityEngine;

public class SpellDamage : MonoBehaviour
{
    public float damage = 10f;
    public LayerMask enemyLayer;
    public bool destroyOnHit = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Debug.Log($"Spell dealt {damage} damage to {other.name}");
            }

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
