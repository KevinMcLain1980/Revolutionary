using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [Header("Arena Settings")]
    [SerializeField] private GameObject gateLeft;
    [SerializeField] private GameObject gateRight;
    [SerializeField] private BossManager bossManager;
    [SerializeField] private AudioClip arenaLockSound;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        gateLeft?.SetActive(true);
        gateRight?.SetActive(true);
        bossManager.enabled = true;

        if (arenaLockSound != null)
            AudioSource.PlayClipAtPoint(arenaLockSound, transform.position);

        Debug.Log("Ember Stag Arena activated");
    }
}
