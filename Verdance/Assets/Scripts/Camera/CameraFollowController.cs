using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowController : MonoBehaviour
{
    private CinemachineCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        FindAndFollowPlayer();
    }

    private void Update()
    {
        if (vcam != null && vcam.Follow == null)
        {
            FindAndFollowPlayer();
        }
    }

    private void FindAndFollowPlayer()
    {
        if (vcam == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            vcam.Follow = player.transform;
            //Debug.Log("Cinemachine camera now following player");
        }
    }
}
