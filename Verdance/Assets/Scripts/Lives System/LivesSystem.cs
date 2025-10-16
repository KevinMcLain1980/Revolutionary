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
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
    }

    public void LoseLife()
    {
        currentLives--;
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            OnGameOver?.Invoke();
        }
    }

    public int GetLives() => currentLives;
}
