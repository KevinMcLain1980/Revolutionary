using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 knockbackDirection = default);
    bool IsDead();
}
