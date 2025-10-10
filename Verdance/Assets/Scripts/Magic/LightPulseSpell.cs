using UnityEngine;

[CreateAssetMenu(fileName = "LightPulse", menuName = "Magic/Spell/Light Pulse")]
public class LightPulseSpell : MagicSpell
{
    public float pulseRadius = 5f;
    public LayerMask enemyLayer;

    public override void Cast(Vector3 position, Vector3 direction)
    {
        if (spellEffectPrefab != null)
        {
            GameObject effect = Instantiate(spellEffectPrefab, position, Quaternion.identity);
            LightPulse lightPulse = effect.GetComponent<LightPulse>();
            if (lightPulse != null)
            {
                lightPulse.Activate();
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, pulseRadius, enemyLayer);
        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
