using UnityEngine;
using UnityEngine.UI;

public class LivesUIManager : MonoBehaviour
{
    [SerializeField] private Text livesText;
    private LivesSystem livesSystem;

    private void Start()
    {
        livesSystem = FindObjectOfType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnLivesChanged += UpdateLivesDisplay;
            UpdateLivesDisplay(livesSystem.GetLives());
        }
    }

    private void UpdateLivesDisplay(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }
}
