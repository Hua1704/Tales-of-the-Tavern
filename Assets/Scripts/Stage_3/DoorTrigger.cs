using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public AudioSource doorSound;
    public AudioSource roaringSound;
    public CutsceneManager cutsceneManager;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return; // prevent multiple triggers
        if (other.CompareTag("Player"))
        {
            triggered = true;

            doorSound.Play();

            // Stop the roaring sound
            if (roaringSound != null)
                roaringSound.Stop();

            // Start the cutscene
            if (cutsceneManager != null)
            {
                cutsceneManager.PlayCutscene();
            }
            else
            {
                Debug.LogError("CutsceneManager not assigned in Inspector!");
            }
        }
    }
}