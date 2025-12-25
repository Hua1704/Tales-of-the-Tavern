using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public HashSet<int> completedStages;
    public HashSet<string> talkedToNpcs;

    // Constructor for a new game
    public GameData()
    {
        completedStages = new HashSet<int>();
        talkedToNpcs = new HashSet<string>();
    }
}