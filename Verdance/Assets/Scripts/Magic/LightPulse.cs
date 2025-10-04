using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public float pulseRadius = 5f;
    public LayerMask enemyLayer;
    public ParticleSystem pulseEffect;
    public AudioClip pulseSound;

    public void Activate()
    {
        if (pulseEffect != null) pulseEffect.Play();
        AudioSource.PlayClipAtPoint(pulseSound, transform.position);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pulseRadius, enemyLayer);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<IStunnable>();
            enemy?.Stun(2f); // Stun for 2 seconds
        }

        Debug.Log("Light Pulse activated");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pulseRadius);
    }
}
