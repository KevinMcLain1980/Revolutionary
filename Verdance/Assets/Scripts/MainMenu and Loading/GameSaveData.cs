using System;

[Serializable]
public class GameSaveData
{
    public string currentLevel;
    public string nextLevel;
    public float playerHealth;
    public float playerSanity;
    public float playerMagic;
    public string saveTime;
    public int levelsCompleted;
}
