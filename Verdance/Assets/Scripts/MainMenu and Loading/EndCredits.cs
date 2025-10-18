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
    [SerializeField] private TMP_Text creditsTextComponent;
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float endPosition = 2000f;
    [SerializeField] private float delayBeforeReturn = 3f;
    [SerializeField] private float initialDelay = 1f;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup creditsCanvasGroup;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("Skip Settings")]
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private CanvasGroup skipCanvasGroup;
    [SerializeField] private string skipMessage = "Press ESC to skip";
    [SerializeField] private float skipTextPulseSpeed = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource creditsMusic;
    [SerializeField] private float musicFadeOutDuration = 2f;

    [Header("Return To")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool creditsComplete = false;
    private bool isSkipping = false;
    private bool hasStarted = false;
    private float skipPulseTimer = 0f;

    private void Start()
    {
        if (skipText != null)
        {
            skipText.text = skipMessage;
        }

        if (creditsCanvasGroup != null)
        {
            creditsCanvasGroup.alpha = 0f;
        }

        if (creditsTextComponent != null)
        {
            FormatCreditsText();
        }

        StartCoroutine(InitializeCredits());
    }

    private IEnumerator InitializeCredits()
    {
        yield return new WaitForSeconds(initialDelay);

        if (creditsCanvasGroup != null)
        {
            yield return StartCoroutine(FadeIn(creditsCanvasGroup, fadeInDuration));
        }

        hasStarted = true;
    }

    private void Update()
    {
        if (!creditsComplete && !isSkipping && hasStarted)
        {
            ScrollCredits();
            CheckSkipInput();
            AnimateSkipText();
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

    private void AnimateSkipText()
    {
        if (skipCanvasGroup == null) return;

        skipPulseTimer += Time.deltaTime * skipTextPulseSpeed;
        skipCanvasGroup.alpha = 0.5f + Mathf.Sin(skipPulseTimer) * 0.5f;
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
        ////Debug.Log("Credits skipped");
        StartCoroutine(FadeOutAndReturn());
    }

    private void OnCreditsComplete()
    {
        creditsComplete = true;
        ////Debug.Log("Credits complete");

        if (skipText != null)
        {
            skipText.text = "Returning to main menu...";
        }

        StartCoroutine(FadeOutAndReturn());
    }

    private IEnumerator FadeOutAndReturn()
    {
        if (creditsMusic != null && creditsMusic.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }

        if (creditsCanvasGroup != null)
        {
            yield return StartCoroutine(FadeOut(creditsCanvasGroup, fadeOutDuration));
        }
        else
        {
            yield return new WaitForSeconds(delayBeforeReturn);
        }

        ReturnToMainMenu();
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOutMusic()
    {
        if (creditsMusic == null) yield break;

        float startVolume = creditsMusic.volume;
        float elapsed = 0f;

        while (elapsed < musicFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            creditsMusic.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeOutDuration);
            yield return null;
        }

        creditsMusic.Stop();
    }

    private void FormatCreditsText()
    {
        string formattedCredits = @"<size=80><b><color=#FFD700>THE END</color></b></size>

<size=50><b><color=#87CEEB>Development Team</color></b></size>

<size=40><color=#FFFFFF>User Interface and Audio</color></size>
<size=35><color=#D3D3D3>Sarah Vanbrocklin</color></size>

<size=40><color=#FFFFFF>Level Design</color></size>
<size=35><color=#D3D3D3>Imani and Kevin</color></size>

<size=40><color=#FFFFFF>Scripting</color></size>
<size=35><color=#D3D3D3>Kevin and Sarah</color></size>

<size=40><color=#FFFFFF>Enemy Design</color></size>
<size=35><color=#D3D3D3>Alex and Kevin</color></size>

<size=40><color=#FFFFFF>Player Design</color></size>
<size=35><color=#D3D3D3>Alex</color></size>

<size=40><color=#FFFFFF>Weapon Design</color></size>
<size=35><color=#D3D3D3>Alex</color></size>


<size=50><b><color=#87CEEB>Unity Asset Store Assets</color></b></size>

<size=35><color=#FFFFFF><b>Nature Sound Effects</b></color></size>
<size=30><color=#D3D3D3>lumino</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/audio/sound-fx/nature-sound-fx-180413</color></size>

<size=35><color=#FFFFFF><b>JRPG/RPG Combat Exploration Theme</b></color></size>
<size=30><color=#D3D3D3>Venblade Studio</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/jrpg-rpg-combat-exploration-theme-307427</color></size>

<size=35><color=#FFFFFF><b>Dark Fantasy Music Pack 2 (mini-Pack)</b></color></size>
<size=30><color=#D3D3D3>Griffin Sibley</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/dark-fantasy-music-pack-2-mini-pack-124182</color></size>

<size=35><color=#FFFFFF><b>Free Parallax Forest background HQ</b></color></size>
<size=30><color=#D3D3D3>Digital Moons</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/free-parallax-forest-background-hq-158680</color></size>

<size=35><color=#FFFFFF><b>Low Poly: Woods Lifestyle</b></color></size>
<size=30><color=#D3D3D3>Rad-Coders</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/low-poly-woods-lifestyle-65306</color></size>

<size=35><color=#FFFFFF><b>Pixel Art Woods Tileset and Background</b></color></size>
<size=30><color=#D3D3D3>karsiori</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/pixel-art-woods-tileset-and-background-280066</color></size>

<size=35><color=#FFFFFF><b>Pixel Dark Forest</b></color></size>
<size=30><color=#D3D3D3>Szadi Art.</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/pixel-dark-forest-136825</color></size>

<size=35><color=#FFFFFF><b>Pixel Fantasy Caves</b></color></size>
<size=30><color=#D3D3D3>Szadi Art.</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/pixel-fantasy-caves-152375</color></size>

<size=35><color=#FFFFFF><b>RPG Essentials - Icon Free Pack (25 Items)</b></color></size>
<size=30><color=#D3D3D3>Pegasus Studios</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/p/rpg-essentials-icon-free-pack-25-items-312684</color></size>

<size=35><color=#FFFFFF><b>RPG Essentials Sound effects</b></color></size>
<size=30><color=#D3D3D3>Leohpaz</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/packages/audio/sound-fx/rpg-essentials-sound-effects-free-227708</color></size>

<size=35><color=#FFFFFF><b>Horror Ambient Album - 060319</b></color></size>
<size=30><color=#D3D3D3>GWriterStudio</color></size>
<size=25><color=#A9A9A9>assetstore.unity.com/publishers/27610</color></size>

<size=35><color=#FFFFFF><b>Main Menu and End Credits Audio - Music</b></color></size>
<size=30><color=#D3D3D3>(Need to find the name again)</color></size>


<size=50><b><color=#87CEEB>Additional Assets</color></b></size>

<size=35><color=#FFFFFF><b>Font, Main menu, Pause and Gameover images</b></color></size>
<size=30><color=#D3D3D3>Initially created by ChatGPT</color></size>
<size=30><color=#D3D3D3>then edited in Adobe Photoshop</color></size>


<size=60><b><color=#FFD700>Thank You For Playing!</color></b></size>";

        creditsTextComponent.text = formattedCredits;
    }

    private void ReturnToMainMenu()
    {
        //Debug.Log($"Returning to {mainMenuSceneName}");
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
