using UnityEngine;

public class LightGateTrigger : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] private GameObject gateObject;
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private string openTriggerName = "Open";

    [Header("Puzzle Settings")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private ParticleSystem activationEffect;
    [SerializeField] private AudioClip activationSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        // Check if the collider is tagged as a Light Pulse
        if (other.CompareTag("LightPulse"))
        {
            ActivateGate();
        }
    }

    private void ActivateGate()
    {
        isActivated = true;

        if (activationEffect != null) activationEffect.Play();
        if (activationSound != null) AudioSource.PlayClipAtPoint(activationSound, transform.position);

        if (gateAnimator != null)
        {
            gateAnimator.SetTrigger(openTriggerName);
        }
        else if (gateObject != null)
        {
            gateObject.SetActive(false); // fallback: disable gate
        }

        Debug.Log("Light Gate activated");
    }
}
