using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    private void Start()
    {
        LivesSystem livesSystem = FindObjectOfType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnGameOver += TriggerGameOver;
        }
    }

    public void TriggerGameOver()
    {
        Debug.Log("Game Over triggered.");
        // Show Game Over screen, disable input, etc.
    }
}
