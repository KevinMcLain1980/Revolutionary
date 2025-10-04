using UnityEngine;

public class WindTunnelForce : MonoBehaviour
{
    [Header("Force Settings")]
    [SerializeField] private Vector2 forceDirection = Vector2.right;
    [SerializeField] private float forceStrength = 10f;
    [SerializeField] private float forceDuration = 2f;

    [Header("Tunnel Settings")]
    [SerializeField] private ParticleSystem windEffect;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private bool oneTimeUse = false;
    private bool hasActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated && oneTimeUse) return;

        var rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && other.CompareTag("Player"))
        {
            StartCoroutine(ApplyWindForce(rb));
            if (windEffect != null) windEffect.Play();
            if (windSound != null) AudioSource.PlayClipAtPoint(windSound, transform.position);
            hasActivated = true;
        }
    }

    private System.Collections.IEnumerator ApplyWindForce(Rigidbody2D rb)
    {
        float timer = 0f;
        while (timer < forceDuration)
        {
            rb.AddForce(forceDirection.normalized * forceStrength, ForceMode2D.Force);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
