using UnityEngine;

public class SceneBoundary : MonoBehaviour
{
    [SerializeField] private bool isLeftBoundary = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.linearVelocity;

                if (isLeftBoundary && velocity.x < 0f)
                    rb.linearVelocity = new Vector2(0f, velocity.y);

                if (!isLeftBoundary && velocity.x > 0f)
                    rb.linearVelocity = new Vector2(0f, velocity.y);
            }
        }
    }
}
