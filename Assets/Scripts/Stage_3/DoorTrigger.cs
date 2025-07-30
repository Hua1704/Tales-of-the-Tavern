using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    public AudioSource doorSound;
    public AudioSource roaringSound;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;

            doorSound.Play();

            if (roaringSound != null)
                roaringSound.Stop();

            StartCoroutine(FreezeGameAfterSound());
        }
    }

    IEnumerator FreezeGameAfterSound()
    {
        yield return new WaitForSeconds(doorSound.clip.length); // wait for the door sound to finish
        Time.timeScale = 0f;
    }
}