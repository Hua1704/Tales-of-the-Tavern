using UnityEngine;
using System.IO;

public static class SaveSystem
{
   
    public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    private static readonly string SAVE_FILE = "save.json";

    public static void Init()
    {
        // Create the save folder if it doesn't exist
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true); // 'true' for pretty print
        File.WriteAllText(SAVE_FOLDER + SAVE_FILE, json);
        Debug.Log("Game Saved!");
    }

    public static GameData LoadGame()
    {
        string filePath = SAVE_FOLDER + SAVE_FILE;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded!");
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating a new game state.");
            return new GameData(); // Return a fresh, empty game state
        }
    }
}