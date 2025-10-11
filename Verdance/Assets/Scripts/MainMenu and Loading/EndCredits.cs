using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class EndCredits : MonoBehaviour
{
    [Header("Credits Settings")]
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float endPosition = 2000f;
    [SerializeField] private float delayBeforeReturn = 3f;

    [Header("Skip Settings")]
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private string skipMessage = "Press ESC to skip";

    [Header("Return To")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool creditsComplete = false;
    private bool isSkipping = false;

    private void Start()
    {
        if (skipText != null)
        {
            skipText.text = skipMessage;
        }
    }

    private void Update()
    {
        if (!creditsComplete && !isSkipping)
        {
            ScrollCredits();
            CheckSkipInput();
        }
    }

    private void ScrollCredits()
    {
        if (creditsText == null) return;

        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsText.anchoredPosition.y >= endPosition)
        {
            OnCreditsComplete();
        }
    }

    private void CheckSkipInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            SkipCredits();
        }
    }

    private void SkipCredits()
    {
        if (isSkipping) return;

        isSkipping = true;
        Debug.Log("Credits skipped");
        ReturnToMainMenu();
    }

    private void OnCreditsComplete()
    {
        creditsComplete = true;
        Debug.Log("Credits complete");

        if (skipText != null)
        {
            skipText.text = "Returning to main menu...";
        }

        Invoke(nameof(ReturnToMainMenu), delayBeforeReturn);
    }

    private void ReturnToMainMenu()
    {
        Debug.Log($"Returning to {mainMenuSceneName}");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
    }

    public void ForceReturnToMenu()
    {
        ReturnToMainMenu();
    }
}
