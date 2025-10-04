using UnityEngine;

public class LightPulseProjectile : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.5f); // auto-cleanup
    }
}
