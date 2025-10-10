using UnityEngine;

[CreateAssetMenu(fileName = "LightProjectile", menuName = "Magic/Spell/Light Projectile")]
public class LightProjectileSpell : MagicSpell
{
    public float projectileSpeed = 10f;

    public override void Cast(Vector3 position, Vector3 direction)
    {
        if (spellEffectPrefab != null)
        {
            GameObject projectile = Instantiate(spellEffectPrefab, position, Quaternion.identity);

            SpellDamage spellDamage = projectile.GetComponent<SpellDamage>();
            if (spellDamage != null)
            {
                spellDamage.damage = damage;
            }

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * projectileSpeed;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
