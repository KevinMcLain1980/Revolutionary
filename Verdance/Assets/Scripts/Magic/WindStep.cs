using UnityEngine;

public class WindStep : MonoBehaviour
{
    public float speedBoost = 2f;
    public float duration = 3f;
    public ParticleSystem windEffect;
    public AudioClip windSound;

    private PlayerController2D player;

    private void Awake()
    {
        player = GetComponent<PlayerController2D>();
    }

    public void Activate()
    {
        if (player == null) return;

        if (windEffect != null) windEffect.Play();
        AudioSource.PlayClipAtPoint(windSound, transform.position);

        StartCoroutine(BoostRoutine());
        Debug.Log("Wind Step activated");
    }

    private System.Collections.IEnumerator BoostRoutine()
    {
        player.ModifySpeed(speedBoost);
        player.SetPhaseThrough(true);

        yield return new WaitForSeconds(duration);

        player.ModifySpeed(1f); // Reset to normal
        player.SetPhaseThrough(false);
    }
}
