using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    private SettingsData currentSettings;
    private static string SettingsPath => Path.Combine(Application.persistentDataPath, "settings.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                currentSettings = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log("Settings loaded");
            }
            else
            {
                currentSettings = SettingsData.GetDefault();
                SaveSettings();
            }

            ApplySettings();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
            currentSettings = SettingsData.GetDefault();
        }
    }

    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(currentSettings, true);
            File.WriteAllText(SettingsPath, json);
            Debug.Log("Settings saved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    private void ApplySettings()
    {
        SetMasterVolume(currentSettings.masterVolume);
        SetMusicVolume(currentSettings.musicVolume);
        SetSFXVolume(currentSettings.sfxVolume);
        SetUIVolume(currentSettings.uiVolume);
    }

    public void SetMasterVolume(float volume)
    {
        currentSettings.masterVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", VolumeToDecibels(volume));
        }
    }

    public void SetMusicVolume(float volume)
    {
        currentSettings.musicVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", VolumeToDecibels(volume));
        }
    }

    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", VolumeToDecibels(volume));
        }
    }

    public void SetUIVolume(float volume)
    {
        currentSettings.uiVolume = volume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("UIVolume", VolumeToDecibels(volume));
        }
    }

    private float VolumeToDecibels(float volume)
    {
        return volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
    }

    public void ResetToDefaults()
    {
        currentSettings = SettingsData.GetDefault();
        ApplySettings();
        SaveSettings();
        Debug.Log("Settings reset to defaults");
    }

    public SettingsData GetCurrentSettings() => currentSettings;

    public float GetMasterVolume() => currentSettings.masterVolume;
    public float GetMusicVolume() => currentSettings.musicVolume;
    public float GetSFXVolume() => currentSettings.sfxVolume;
    public float GetUIVolume() => currentSettings.uiVolume;
}
