using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string saveFilePath = Application.persistentDataPath + "/progress.json";

    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game progress saved to: " + saveFilePath);
    }

    public static GameData Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null; // No save file exists
    }
}