using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    private void Start()
    {
        LivesSystem livesSystem = FindObjectOfType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnGameOver += TriggerGameOver;
            Debug.Log("[GameOverManager] Subscribed to GameOver event.");
        }
        else
        {
            Debug.LogWarning("[GameOverManager] LivesSystem not found.");
        }
    }

    public void TriggerGameOver()
    {
        Debug.Log("[GameOverManager] Game Over triggered.");
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Debug.Log("[GameOverManager] Game Over screen activated.");
        }
        else
        {
            Debug.LogWarning("[GameOverManager] Game Over screen not assigned.");
        }

    }
}
