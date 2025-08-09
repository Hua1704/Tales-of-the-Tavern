using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnEnable : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Background Music to fade out")]
    public AudioSource backgroundMusic;  // Assign your BG music AudioSource here
    public float fadeDuration = 1f;      // Duration to fade out/in BG music

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            StartCoroutine(PlaySoundWithBGFade());
        }
    }

    private IEnumerator PlaySoundWithBGFade()
    {
        // Fade out background music
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            yield return StartCoroutine(FadeAudio(backgroundMusic, backgroundMusic.volume, 0f, fadeDuration));
            backgroundMusic.Pause();
        }

        // Play current sound (game over sound)
        audioSource.Play();

        // Wait for clip to finish
        yield return new WaitForSeconds(audioSource.clip.length);

        // Fade BG music back in
        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
            yield return StartCoroutine(FadeAudio(backgroundMusic, 0f, 1f, fadeDuration));
        }
    }

    // Generic audio fade coroutine
    private IEnumerator FadeAudio(AudioSource source, float fromVolume, float toVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(fromVolume, toVolume, elapsed / duration);
            yield return null;
        }
        source.volume = toVolume;
    }
}