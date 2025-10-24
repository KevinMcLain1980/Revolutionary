using UnityEngine;

public static class SaveSystem
{
    private static string SaveKey = "GameSave";

    public static void SaveGame(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            //Debug.Log($"Game saved to PlayerPrefs");
        }
        catch (System.Exception)
        {
            //Debug.LogError($"Failed to save game:");
        }
    }

    public static GameSaveData LoadGame()
    {
        try
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                string json = PlayerPrefs.GetString(SaveKey);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                //Debug.Log("Game loaded successfully");
                return data;
            }
            else
            {
                //Debug.Log("No save file found");
                return null;
            }
        }
        catch
        {
            //Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    public static bool HasSaveFile()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        //Debug.Log("Save file deleted");
    }
}
