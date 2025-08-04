using UnityEngine;

public static class StageManager
{
    // We will create a PlayerPrefs key for each stage, e.g., "Stage_1_Completed"
    private const string StageCompletionKeyPrefix = "Stage_";
    private const string CompletedSuffix = "_Completed";

    /// <summary>

    /// </summary>
    /// <param name="stageNumber">The number of the stage to complete (e.g., 1 for Stage 1).</param>
    public static void CompleteStage(int stageNumber)
    {
        if (stageNumber <= 0) return;

        string key = StageCompletionKeyPrefix + stageNumber + CompletedSuffix;
        // We save '1' to represent 'true' because PlayerPrefs is simple.
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save(); // Force the save to disk.
        Debug.Log("Stage " + stageNumber + " marked as completed!");
    }

    /// <summary>
    /// Checks if a specific stage has been completed.
    /// </summary>
    /// <param name="stageNumber">The number of the stage to check (e.g., 1 for Stage 1).</param>
    /// <returns>True if the stage is complete, false otherwise.</returns>
    public static bool IsStageCompleted(int stageNumber)
    {
        if (stageNumber <= 0) return false;

        string key = StageCompletionKeyPrefix + stageNumber + CompletedSuffix;
        // GetInt will default to 0 (false) if the key has never been saved.
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    /// <summary>
    /// A helper function for development to reset all stage progress.
    /// </summary>
    public static void ResetAllStageProgress()
    {
     
        PlayerPrefs.DeleteAll();
       
        Debug.LogWarning("All PlayerPrefs (including all stage progress) have been reset!");
    }
}