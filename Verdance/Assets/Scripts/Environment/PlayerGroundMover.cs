using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundMover : MonoBehaviour
{
    [SerializeField] private GroundConnector currentGround;
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            TryMoveTo(currentGround.leftGround);
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            TryMoveTo(currentGround.rightGround);
        }
    }

    private void TryMoveTo(GroundConnector target)
    {
        if (target != null && target.isPlayerTouching)
        {
            transform.position = target.transform.position;
            currentGround = target;
            Debug.Log($"Moved to {target.name}");
        }
    }
}
