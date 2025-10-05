using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Attack Phases")]
    [SerializeField] private BossPhase[] phases;
    private int currentPhaseIndex = 0;

    [Header("Feedback")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem damageEffect;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Arena Logic")]
    [SerializeField] private GameObject arenaGate;
    [SerializeField] private string deathTrigger = "Die";

    private void Awake()
    {
        currentHealth = maxHealth;
        if (phases.Length > 0)
        {
            phases[0].ActivatePhase();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (damageEffect != null) damageEffect.Play();
        if (damageSound != null) AudioSource.PlayClipAtPoint(damageSound, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            CheckPhaseTransition();
        }
    }

    private void CheckPhaseTransition()
    {
        if (currentPhaseIndex < phases.Length - 1 && currentHealth <= phases[currentPhaseIndex + 1].triggerHealth)
        {
            currentPhaseIndex++;
            phases[currentPhaseIndex].ActivatePhase();
            Debug.Log($"Boss transitioned to phase {currentPhaseIndex}");
        }
    }

    private void Die()
    {
        animator?.SetTrigger(deathTrigger);
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);
        if (arenaGate != null) arenaGate.SetActive(false); // unlock arena
        Debug.Log("Boss defeated");
        Destroy(gameObject, 3f); // optional cleanup
    }
}
