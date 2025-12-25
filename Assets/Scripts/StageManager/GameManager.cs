using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameData CurrentProgress { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadProgress();
    }

    private void LoadProgress()
    {
        CurrentProgress = SaveManager.Load();
        if (CurrentProgress == null)
        {
            Debug.Log("No save file found. Initializing new game progress.");
            CurrentProgress = new GameData();
        }
    }

    public void MarkStageAsCompleted(int stageNumber)
    {
        if (CurrentProgress.completedStages.Add(stageNumber)) 
        {
            SaveManager.Save(CurrentProgress);
        }
    }

    public bool IsStageCompleted(int stageNumber)
    {
        return CurrentProgress.completedStages.Contains(stageNumber);
    }

    public void MarkNpcAsTalkedTo(string npcId)
    {
        if (CurrentProgress.talkedToNpcs.Add(npcId))
        {
            SaveManager.Save(CurrentProgress);
        }
    }

    public bool HasNpcBeenTalkedTo(string npcId)
    {
        return CurrentProgress.talkedToNpcs.Contains(npcId);
    }
}