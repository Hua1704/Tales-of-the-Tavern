using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnEnable : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // Play the audio clip as a one-shot sound
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}