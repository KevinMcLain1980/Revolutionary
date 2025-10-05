using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Camera Target")]
    public Transform cameraTarget; // The object Cinemachine Camera follows

    [Header("Trigger Zone Settings")]
    public Collider2D triggerZone; // Invisible zone around player
    public float followSpeed = 5f;

    private bool shouldFollow = false;

    void Start()
    {
        if (triggerZone == null)
        {
            Debug.LogWarning("Camera trigger zone not assigned.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == triggerZone)
        {
            shouldFollow = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == triggerZone)
        {
            shouldFollow = false;
        }
    }

    void LateUpdate()
    {
        if (shouldFollow && cameraTarget != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 2f, cameraTarget.position.z);
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, targetPosition, Time.deltaTime * followSpeed);
        }
    }
}
