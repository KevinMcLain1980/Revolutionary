using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float hitboxDuration = 0.2f;

    private void OnEnable()
    {
        Invoke(nameof(DisableHitbox), hitboxDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has a ShamblerAI component
        ShamblerAI shambler = other.GetComponent<ShamblerAI>();
        if (shambler != null)
        {
            shambler.TakeDamage(damageAmount);
        }

        // Optional: support other enemy types
        // var health = other.GetComponent<EnemyHealth>();
        // if (health != null)
        // {
        //     health.TakeDamage(damageAmount);
        // }
    }

    private void DisableHitbox()
    {
        gameObject.SetActive(false);
    }
}
