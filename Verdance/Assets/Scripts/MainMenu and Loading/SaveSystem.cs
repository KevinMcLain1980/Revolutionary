using UnityEngine;
using System.IO;

// Data structure for saving game progress
[System.Serializable]
public class SaveData
{
    public int currentLevel;
    public float playerHealth;
    public float playerStamina;
    public float playerMagic;
    public int cursedObjectsCollected;
}

public class SaveSystem : MonoBehaviour
{
    // Path where save file is stored
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    // Save game progress to file
    public static void SaveGame(int levelIndex, float health = 100f, float stamina = 100f, float magic = 100f, int cursedObjects = 0)
    {
        SaveData data = new SaveData
        {
            currentLevel = levelIndex,
            playerHealth = health,
            playerStamina = stamina,
            playerMagic = magic,
            cursedObjectsCollected = cursedObjects
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game saved! Level: {levelIndex}");
    }

    // Load saved game data
    public static SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Game loaded! Level: {data.currentLevel}");
            return data;
        }

        Debug.LogWarning("No save file found!");
        return null;
    }

    // Check if a save file exists
    public static bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    // Delete the save file
    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted!");
        }
    }
}
