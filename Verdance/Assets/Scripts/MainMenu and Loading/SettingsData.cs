using System;
using System.Collections.Generic;

[Serializable]
public class SettingsData
{
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float uiVolume = 1f;

    public Dictionary<string, string> keybinds = new Dictionary<string, string>();

    public static SettingsData GetDefault()
    {
        return new SettingsData
        {
            masterVolume = 1f,
            musicVolume = 0.7f,
            sfxVolume = 0.8f,
            uiVolume = 0.5f,
            keybinds = new Dictionary<string, string>()
        };
    }
}
