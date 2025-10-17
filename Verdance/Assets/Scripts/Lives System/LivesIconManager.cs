using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LivesIconManager : MonoBehaviour
{
    [Header("Icon Setup")]
    [SerializeField] private GameObject lifeIconPrefab;
    [SerializeField] private Transform iconContainer;

    private List<GameObject> activeIcons = new List<GameObject>();
    private int previousLives = -1;

    private void Start()
    {
        LivesSystem livesSystem = Object.FindFirstObjectByType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnLivesChanged += UpdateIcons;
            int startingLives = livesSystem.GetLives();
            InitializeIcons(startingLives);
            previousLives = startingLives;
        }
        else
        {
            Debug.LogWarning("[LivesIconManager] LivesSystem not found.");
        }
    }

    private void InitializeIcons(int lives)
    {
        for (int i = 0; i < lives; i++)
        {
            GameObject icon = Instantiate(lifeIconPrefab, iconContainer);
            activeIcons.Add(icon);
        }
    }

    private void UpdateIcons(int currentLives)
    {
        if (currentLives < previousLives && activeIcons.Count > 0)
        {
            GameObject iconToRemove = activeIcons[activeIcons.Count - 1];
            activeIcons.RemoveAt(activeIcons.Count - 1);
            Destroy(iconToRemove);
            Debug.Log($"[LivesIconManager] Removed one icon. Lives remaining: {currentLives}");
        }

        previousLives = currentLives;
    }
}
