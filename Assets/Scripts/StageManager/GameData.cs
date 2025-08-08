using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // Dictionary to store which stages are completed. Key: stage name (e.g., "Stage1"), Value: true/false
    public SerializableDictionary<string, bool> stagesCompleted;

    // Dictionary to store NPC interaction flags. Key: unique NPC ID, Value: true/false
    public SerializableDictionary<string, bool> npcInteractedFlags;
    public string currentChapterId;
    public bool isWitchsBossDefeated; // Defaults to false


    // The constructor is called when we create a new game state
    public GameData()
    {
        stagesCompleted = new SerializableDictionary<string, bool>();
        npcInteractedFlags = new SerializableDictionary<string, bool>();
        currentChapterId = "";
        isWitchsBossDefeated = false;
    }
}