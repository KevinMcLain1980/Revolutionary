using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private bool spawnOnStart = true;

    public static PlayerSpawnPoint Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

        if (existingPlayer != null)
        {
            existingPlayer.transform.position = transform.position;
            existingPlayer.transform.rotation = transform.rotation;
            Debug.Log("Player repositioned to spawn point");
        }
        else if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);
            Debug.Log("Player spawned at spawn point");
        }
        else
        {
            Debug.LogWarning("No player found and no player prefab assigned!");
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 1f);
    }
}
