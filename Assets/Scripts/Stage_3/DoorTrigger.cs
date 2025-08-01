using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
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
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.75f;

    [Header("Return Flow")]
    [Tooltip("Scene name or build index to load after the cutscene")]
    public string returnSceneName = "Stage0"; 
    public float holdOnBlackAfterVideo = 0.25f; // small pause before loading (seconds, realtime)

    private bool triggered = false;
    private float prevTimeScale = 1f;
    [Header("Conversation")]
    public ConversationData conversationToPlay;

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
        // prevTimeScale = Time.timeScale;
        // Time.timeScale = 0f;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // 1) Fade to black
        yield return FadeTo(1f, fadeDuration);
           if (conversationToPlay != null)
        {
         
            bool conversationFinished = false;

          
            ConversationPlayer.Instance.StartConversation(conversationToPlay, () => {
                conversationFinished = true;
            });

          
            yield return FadeTo(0f, fadeDuration);

       
            yield return new WaitUntil(() => conversationFinished);
        }
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
            yield return FadeTo(0f, fadeDuration);

            // Wait until the video finishes
            while (cutsceneVideo.isPlaying)
                yield return null;
        }

        // 4) Fade to black again before switching scenes
        yield return FadeTo(1f, fadeDuration);

        // small hold on black (realtime; still paused)
        if (holdOnBlackAfterVideo > 0f)
        {
            float t = 0f;
            while (t < holdOnBlackAfterVideo)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        // stop video (optional, scene load will destroy it anyway)
        if (cutsceneVideo) cutsceneVideo.Stop();

        // 5) Restore timescale so next scene runs normally
        Time.timeScale = prevTimeScale;

        // 6) Load Stage 0
        if (!string.IsNullOrEmpty(returnSceneName))
            SceneManager.LoadScene(returnSceneName);
        else
            Debug.LogError("DoorTrigger: returnSceneName is empty. Set it to your Stage 0 scene name.");
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (!fadeGroup || duration <= 0f) yield break;

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
        fadeGroup.blocksRaycasts = targetAlpha > 0.001f;
    }
}