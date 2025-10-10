using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackStrength = 6f;
    [SerializeField] private float knockbackLift = 4f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var receiver = other.GetComponent<ShamblerAI>();
            if (receiver != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                Vector2 knockback = new Vector2(direction.x * knockbackStrength, knockbackLift);
                receiver.TakeDamage(damageAmount, knockback);
            }
        }
    }
}
