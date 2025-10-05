using UnityEngine;

public class GroundNode : MonoBehaviour
{
    [Header("Connected Nodes")]
    public GroundNode leftNode;
    public GroundNode rightNode;

    [Header("Snap Settings")]
    public Transform snapPoint; // where player should land
}
