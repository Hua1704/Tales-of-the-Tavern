using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject tutorialCanvas;    // Reference to the tutorial UI canvas
    public float displayDuration = 6f;  

    public AudioSource audioSource;

    private bool triggered = false;      // To ensure the tutorial only triggers once

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;            // Don't trigger again if already triggered

        if (other.CompareTag("Player"))  // Check if the player entered the trigger
        {
            triggered = true;
            StartCoroutine(ShowTutorial());
        }
    }

    private IEnumerator ShowTutorial()
    {
        Time.timeScale = 0f;              // Pause the game

        tutorialCanvas.SetActive(true);   // Show the tutorial canvas

        if (audioSource != null)
        {
            audioSource.Play();
        }
        yield return new WaitForSecondsRealtime(displayDuration);

        tutorialCanvas.SetActive(false);  // Hide the tutorial canvas

        Time.timeScale = 1f;              // Resume the game

        gameObject.SetActive(false);      // Disable this trigger so it doesn't activate again
    }
}