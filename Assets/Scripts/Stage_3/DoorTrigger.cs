using UnityEngine;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource roaringSound;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public Rigidbody2D playerRb;

    [Header("Cutscene")]
    public VideoPlayer cutsceneVideo;

    [Header("Fade")]
    public CanvasGroup fadeGroup;   // assign FadedScreen's CanvasGroup here
    public float fadeDuration = 0.75f;

    private bool triggered = false;
    private float prevTimeScale = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        // stop player input & motion
        if (playerMovementScript) playerMovementScript.enabled = false;
#if UNITY_6000_0_OR_NEWER
        if (playerRb) { playerRb.linearVelocity = Vector2.zero; playerRb.angularVelocity = 0f; }
#else
        if (playerRb) { playerRb.velocity = Vector2.zero; playerRb.angularVelocity = 0f; }
#endif

        if (roaringSound && roaringSound.isPlaying) roaringSound.Stop();

        // freeze gameplay
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // 1) Fade to black
        yield return FadeTo(1f, fadeDuration);   // CanvasGroup alpha: 0 -> 1

        // 2) Configure & play video full screen
        if (cutsceneVideo)
        {
            cutsceneVideo.renderMode = VideoRenderMode.CameraNearPlane;
            cutsceneVideo.targetCamera = Camera.main;
            cutsceneVideo.targetCameraAlpha = 1f;
            cutsceneVideo.aspectRatio = VideoAspectRatio.FitVertically;
            cutsceneVideo.playOnAwake = false;
            cutsceneVideo.isLooping = false;

            cutsceneVideo.Prepare();
            while (!cutsceneVideo.isPrepared)
                yield return null;

            cutsceneVideo.Play();

            // 3) Reveal the video (fade from black)
            yield return FadeTo(0f, fadeDuration); // CanvasGroup alpha: 1 -> 0

            // Wait until the video finishes
            while (cutsceneVideo.isPlaying)
                yield return null;
        }

        // OPTIONAL: stay frozen, or resume
        // Time.timeScale = prevTimeScale;
        // if (playerMovementScript) playerMovementScript.enabled = true;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (!fadeGroup || duration <= 0f) yield break;

        // Ensure the overlay blocks clicks during fade
        fadeGroup.blocksRaycasts = true;

        float start = fadeGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;                 // works while timeScale = 0
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
        fadeGroup.blocksRaycasts = targetAlpha > 0.001f; // unblock if fully transparent
    }
}