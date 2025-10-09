using UnityEngine;

public class WindStep : MonoBehaviour
{
    [Header("WindStep Settings")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private bool enablePhaseThrough = true;

    [Header("References")]
    [SerializeField] private PlayerController2D player;

    private bool isActive = false;

    private void Start()
    {
        if (player == null)
            player = GetComponent<PlayerController2D>();
    }

    public void Activate()
    {
        if (isActive || player == null) return;

        isActive = true;

        // Apply speed boost
        player.ModifySpeed(speedMultiplier, duration);

        // Enable phase-through
        if (enablePhaseThrough)
            player.SetPhaseThrough(true);

        // Schedule reset
        Invoke(nameof(Deactivate), duration);
    }

    private void Deactivate()
    {
        if (player != null && enablePhaseThrough)
            player.SetPhaseThrough(false);

        isActive = false;
    }
}
