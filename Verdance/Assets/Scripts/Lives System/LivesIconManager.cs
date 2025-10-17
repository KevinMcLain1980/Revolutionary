using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LivesIconManager : MonoBehaviour
{
    [Header("Icon Setup")]
    [SerializeField] private GameObject lifeIconPrefab;
    [SerializeField] private Transform iconContainer;

    private List<GameObject> activeIcons = new List<GameObject>();

    private void Start()
    {
        LivesSystem livesSystem = Object.FindFirstObjectByType<LivesSystem>();
        if (livesSystem != null)
        {
            livesSystem.OnLivesChanged += UpdateIcons;
            UpdateIcons(livesSystem.GetLives());
            Debug.Log($"[LivesIconManager] Initialized with {livesSystem.GetLives()} lives.");
        }
        else
        {
            Debug.LogWarning("[LivesIconManager] LivesSystem not found.");
        }
    }

    private void UpdateIcons(int lives)
    {
        // Clear existing icons
        foreach (var icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        // Instantiate new icons
        for (int i = 0; i < lives; i++)
        {
            GameObject icon = Instantiate(lifeIconPrefab, iconContainer);
            activeIcons.Add(icon);
        }

        Debug.Log($"[LivesIconManager] Updated icons. Lives remaining: {lives}");
    }
}
