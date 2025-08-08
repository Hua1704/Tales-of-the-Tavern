using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameData gameData;
    public BaseUnit playerInfo;
    public GameObject gameOverScreen;
    void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            // Initialize and load the game
            SaveSystem.Init();
            gameData = SaveSystem.LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public string GetCurrentChapter()
    {
        return gameData.currentChapterId;
    }

    public void SetCurrentChapter(string chapterId)
    {
        gameData.currentChapterId = chapterId;
        SaveSystem.SaveGame(gameData); // Save immediately when the current chapter changes.
    }
    // --- NPC Flag Methods ---
    public bool GetNpcFlag(string npcId)
    {
        gameData.npcInteractedFlags.TryGetValue(npcId, out bool hasInteracted);
        return hasInteracted; // Returns false if the key doesn't exist
    }

    public void SetNpcFlag(string npcId, bool value)
    {
        if (gameData.npcInteractedFlags.ContainsKey(npcId))
        {
            gameData.npcInteractedFlags[npcId] = value;
        }
        else
        {
            gameData.npcInteractedFlags.Add(npcId, value);
        }
        // Save after changing a flag
        SaveSystem.SaveGame(gameData);
    }

    // --- Stage Completion Methods ---
    public bool IsStageComplete(string stageName)
    {
        gameData.stagesCompleted.TryGetValue(stageName, out bool isComplete);
        return isComplete;
    }

    public void MarkStageAsComplete(string stageName)
    {
        if (gameData.stagesCompleted.ContainsKey(stageName))
        {
            gameData.stagesCompleted[stageName] = true;
        }
        else
        {
            gameData.stagesCompleted.Add(stageName, true);
        }
        // Save after completing a stage
        SaveSystem.SaveGame(gameData);
    }
    private void Update()
    {
        if ((playerInfo.health == 0 || playerInfo == null) && gameOverScreen != null && !gameOverScreen.activeSelf)
        {
            Debug.Log("Game Over");
            gameOverScreen.SetActive(true);
        }
    }
    public void OnPressRetry()
    {
        SceneManager.LoadScene("Stage0");
    }
    public void OnPressMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void SetWitchsBossDefeated(bool status)
    {
        if (gameData != null)
        {
            gameData.isWitchsBossDefeated = status;
            SaveSystem.SaveGame(gameData); // Save the progress immediately
            Debug.Log("Witch's Boss Defeated status saved!");
        }
    }

    public bool IsWitchsBossDefeated()
    {
        if (gameData != null)
        {
            return gameData.isWitchsBossDefeated;
        }
        return false; 
    }
}