using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorTrigger : MonoBehaviour
{
    // --- All Headers Combined for a Clean Inspector ---

    [Header("Sequence Configuration")]
    [Tooltip("(Optional) A conversation to play before the video.")]
    public ConversationData conversationToPlay;
    [Tooltip("(Optional) A video to play after the conversation.")]
    public VideoPlayer cutsceneVideo;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public Rigidbody2D playerRb;

    [Header("Fade Effect")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.75f;

    [Header("Audio")]
    public AudioSource roaringSound;

    [Header("Final Action")]
    [Tooltip("The scene to load after the sequence is complete.")]
    public string returnSceneName = "Stage0";
    public float holdOnBlackAfterSequence = 0.25f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;

        // --- Initial Setup ---
        // Disable player movement immediately.
        if (playerMovementScript) playerMovementScript.enabled = false;
        if (playerRb) { playerRb.linearVelocity = Vector2.zero; playerRb.angularVelocity = 0f; }

        // Stop any associated sounds.
        if (roaringSound && roaringSound.isPlaying) roaringSound.Stop();

        // Start the full sequence.
        StartCoroutine(PlayFullSequence());
    }

    private IEnumerator PlayFullSequence()
    {
        // --- Part 1: Fade to Black ---
        yield return FadeTo(1f, fadeDuration);

        // --- Part 2: Play Conversation (if assigned) ---
        if (conversationToPlay != null)
        {
            // The conversation system needs Time.timeScale = 1 to work.
            // We ensure it's not paused here.
            Time.timeScale = 1f;

            // Fade back in so the player can see the dialogue UI.
            yield return FadeTo(0f, fadeDuration);

            bool conversationFinished = false;
            ConversationPlayer.Instance.StartConversation(conversationToPlay, () => {
                conversationFinished = true;
            });

            // Wait here until the conversation signals it's over.
            yield return new WaitUntil(() => conversationFinished);

            // Fade back to black before the next step.
            yield return FadeTo(1f, fadeDuration);
        }

        // --- Part 3: Play Video (if assigned) ---
        if (cutsceneVideo != null)
        {
            // Now we pause the game for the video cutscene.
            float prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            // Configure and prepare the video.
            cutsceneVideo.renderMode = VideoRenderMode.CameraNearPlane;
            cutsceneVideo.targetCamera = Camera.main;
            cutsceneVideo.targetCameraAlpha = 1f;
            cutsceneVideo.Prepare();
            while (!cutsceneVideo.isPrepared)
                yield return null;

            // Play the video and fade it in.
            cutsceneVideo.Play();
            yield return FadeTo(0f, fadeDuration);

            // Wait here until the video finishes playing.
            while (cutsceneVideo.isPlaying)
                yield return null;

            // Stop the video and fade back to black.
            cutsceneVideo.Stop();
            yield return FadeTo(1f, fadeDuration);

            
            Time.timeScale = prevTimeScale;
        }

 
        if (holdOnBlackAfterSequence > 0f)
        {
            yield return new WaitForSecondsRealtime(holdOnBlackAfterSequence);
        }

        // Ensure time is fully restored before loading the next scene.
        Time.timeScale = 1f;

        // Load the return scene.
        if (!string.IsNullOrEmpty(returnSceneName))
            SceneManager.LoadScene(returnSceneName);
        else
            Debug.LogError("DoorTrigger: returnSceneName is empty!", this.gameObject);
    }

    // Your FadeTo coroutine is perfect and doesn't need changes.
    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (!fadeGroup) yield break;
        fadeGroup.blocksRaycasts = true;
        float start = fadeGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        fadeGroup.alpha = targetAlpha;
        fadeGroup.blocksRaycasts = targetAlpha > 0.001f;
    }
}