using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource backgroundMusic;
    public AudioSource battleMusic;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBattleMusic()
    {
        StartCoroutine(SwitchMusic(backgroundMusic, battleMusic));
    }

    public void PlayBackgroundMusic()
    {
        StartCoroutine(SwitchMusic(battleMusic, backgroundMusic));
    }

    private IEnumerator SwitchMusic(AudioSource from, AudioSource to)
    {
        // Fade out "from"
        if (from != null && from.isPlaying)
        {
            float startVolume = from.volume;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                from.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            from.Stop();
            from.volume = startVolume;
        }

        // Fade in "to"
        if (to != null)
        {
            to.volume = 0f;
            to.Play();

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                to.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
            to.volume = 1f;
        }
    }
}