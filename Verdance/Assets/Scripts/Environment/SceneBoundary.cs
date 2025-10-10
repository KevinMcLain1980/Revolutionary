using UnityEngine;

public class SceneBoundary : MonoBehaviour
{
    [SerializeField] private bool isLeftBoundary = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 velocity = rb.linearVelocity;

        // Block movement INTO the boundary only
        if (isLeftBoundary && velocity.x < 0f)
        {
            rb.linearVelocity = new Vector2(0f, velocity.y);
        }
        else if (!isLeftBoundary && velocity.x > 0f)
        {
            rb.linearVelocity = new Vector2(0f, velocity.y);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // No action needed—player can freely exit the trigger
    }
}
