using UnityEngine;
using System;

public class LivesSystem : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    public event Action<int> OnLivesChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        currentLives = maxLives;
        Debug.Log($"[LivesSystem] Initialized with {currentLives} lives.");
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        Debug.Log($"[LivesSystem] Lives reset to {currentLives}.");
        OnLivesChanged?.Invoke(currentLives);
    }

    public void LoseLife()
    {
        currentLives--;
        Debug.Log($"[LivesSystem] Life lost. Remaining lives: {currentLives}");
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("[LivesSystem] No lives remaining. Triggering Game Over.");
            OnGameOver?.Invoke();
        }
    }

    public int GetLives()
    {
        Debug.Log($"[LivesSystem] GetLives called. Current lives: {currentLives}");
        return currentLives;
    }
}
