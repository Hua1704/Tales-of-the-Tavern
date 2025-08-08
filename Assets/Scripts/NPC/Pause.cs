using UnityEngine;

public class Pause : MonoBehaviour
{
    
    public static Pause Instance { get; private set; }

    public bool IsGamePaused { get; private set; }

    void Awake()
    {
        // Set up the singleton instance.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void SetPause(bool shouldPause)
    {
        if (shouldPause)
        {
            Time.timeScale = 0f;
            IsGamePaused = true;
            Debug.Log("Game Paused.");
        }
        else
        {
            Time.timeScale = 1f;
            IsGamePaused = false;
            Debug.Log("Game Resumed.");
        }
    }
}