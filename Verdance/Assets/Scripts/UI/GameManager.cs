using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text cursedObjectText;
    [SerializeField] private Slider sanitySlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider magicSlider;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private int cursedObjectsCollected = 0;
    private float currentSanity = 1f;
    private float currentHealth = 1f;
    private float currentMagic = 1f;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Cursed Object UI
    public void UpdateCursedObjectCount(int count)
    {
        cursedObjectsCollected = count;
        if (cursedObjectText != null)
            cursedObjectText.text = $"Cursed Objects: {cursedObjectsCollected}";
    }

    // Sanity UI
    public void UpdateSanity(float sanity)
    {
        currentSanity = Mathf.Clamp01(sanity);
        if (sanitySlider != null)
            sanitySlider.value = currentSanity;
    }

    // Health UI
    public void UpdateHealth(float health)
    {
        currentHealth = Mathf.Clamp01(health);
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    // Magic UI
    public void UpdateMagic(float magic)
    {
        currentMagic = Mathf.Clamp01(magic);
        if (magicSlider != null)
            magicSlider.value = currentMagic;
    }

    // Game State
    public void TriggerWin()
    {
        if (winScreen != null)
            winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerLose()
    {
        if (loseScreen != null)
            loseScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
