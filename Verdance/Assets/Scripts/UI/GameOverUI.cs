using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Hover Visual Feedback")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverAnimationSpeed = 0.15f;
    [SerializeField] private Color hoverTintColor = new Color(1f, 1f, 0.7f, 1f);

    private System.Collections.Generic.Dictionary<Button, ButtonState> buttonOriginalStates = new System.Collections.Generic.Dictionary<Button, ButtonState>();
    private System.Collections.Generic.Dictionary<Button, Coroutine> activeButtonCoroutines = new System.Collections.Generic.Dictionary<Button, Coroutine>();

    private struct ButtonState
    {
        public Color originalColor;
        public Vector3 originalScale;
    }

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestart);
            AddHoverEffect(restartButton);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettings);
            AddHoverEffect(settingsButton);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenu);
            AddHoverEffect(mainMenuButton);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void AddHoverEffect(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = new ButtonState
        {
            originalColor = buttonImage != null ? buttonImage.color : Color.white,
            originalScale = button.transform.localScale
        };
        buttonOriginalStates[button] = originalState;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { OnButtonHoverEnter(button); });
        trigger.triggers.Add(hoverEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnButtonHoverExit(button); });
        trigger.triggers.Add(exitEntry);
    }

    private void OnButtonHoverEnter(Button button)
    {
        if (!button.interactable || !gameObject.activeInHierarchy) return;

        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, true));
    }

    private void OnButtonHoverExit(Button button)
    {
        if (!button.interactable || !gameObject.activeInHierarchy) return;

        if (activeButtonCoroutines.ContainsKey(button) && activeButtonCoroutines[button] != null)
            StopCoroutine(activeButtonCoroutines[button]);

        activeButtonCoroutines[button] = StartCoroutine(AnimateButtonHover(button, false));
    }

    private System.Collections.IEnumerator AnimateButtonHover(Button button, bool isHovering)
    {
        if (!buttonOriginalStates.ContainsKey(button)) yield break;

        Transform buttonTransform = button.transform;
        Image buttonImage = button.GetComponent<Image>();
        ButtonState originalState = buttonOriginalStates[button];

        Vector3 targetScale = isHovering ? originalState.originalScale * hoverScale : originalState.originalScale;
        Color targetColor = isHovering ? hoverTintColor : originalState.originalColor;

        Vector3 startScale = buttonTransform.localScale;
        Color startColor = buttonImage != null ? buttonImage.color : originalState.originalColor;

        float elapsed = 0f;

        while (elapsed < hoverAnimationSpeed)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / hoverAnimationSpeed;

            buttonTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (buttonImage != null)
                buttonImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        buttonTransform.localScale = targetScale;
        if (buttonImage != null)
            buttonImage.color = targetColor;
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;

        PlayerRespawn respawn = FindFirstObjectByType<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.ResetLives();
        }

        PlayerStats stats = PlayerStats.Instance;
        if (stats != null)
        {
            stats.SetHealth(stats.GetMaxHealth());
            stats.SetSanity(stats.GetMaxSanity());
            stats.SetMagic(stats.GetMaxMagic());
        }

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.ResetInventory();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
