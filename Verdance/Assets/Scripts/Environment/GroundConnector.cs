using UnityEngine;

public class GroundConnector : MonoBehaviour
{
    [Header("Connected Ground")]
    public GroundConnector leftGround;
    public GroundConnector rightGround;

    [HideInInspector] public bool isPlayerTouching = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerTouching = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerTouching = false;
    }
}
