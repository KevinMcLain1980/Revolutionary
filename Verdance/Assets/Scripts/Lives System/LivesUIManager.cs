using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    private LivesSystem livesSystem;

    private void Start()
    {
        LivesSystem livesSystem = Object.FindAnyObjectByType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnLivesChanged += UpdateLivesDisplay;
            int initialLives = livesSystem.GetLives();
            UpdateLivesDisplay(initialLives);
            Debug.Log($"[LivesUIManager] Subscribed to LivesSystem. Initial lives: {initialLives}");
        }
        else
        {
            Debug.LogWarning("[LivesUIManager] LivesSystem not found.");
        }
    }

    private void UpdateLivesDisplay(int lives)
    {
        livesText.text = $"Lives: {lives}";
        Debug.Log($"[LivesUIManager] UI updated. Lives: {lives}");
    }
}
