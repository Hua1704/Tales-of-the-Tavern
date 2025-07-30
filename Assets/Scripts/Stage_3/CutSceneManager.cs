using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public Image fadeImage;                        // Black background image
    public TextMeshProUGUI cutsceneText;           // Text to display the message
    public float fadeDuration = 2f;                // Time to fade in

    public void PlayCutscene()
    {
        // Ensure fade image starts invisible (alpha = 0)
        if (fadeImage != null)
        {
            Color startColor = fadeImage.color;
            startColor.a = 0f;
            fadeImage.color = startColor;
        }

        // Ensure cutscene text is visible and cleared
        if (cutsceneText != null)
        {
            cutsceneText.text = "";
            Color textColor = cutsceneText.color;
            textColor.a = 1f; // Fully visible
            cutsceneText.color = textColor;
        }

        StartCoroutine(FadeAndShowText());
    }

    IEnumerator FadeAndShowText()
    {
        // Step 1: Fade to black
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        // Step 2: Pause briefly
        yield return new WaitForSeconds(1f);

        // Step 3: Display cutscene text step by step
        cutsceneText.text = "";
        yield return new WaitForSeconds(0.5f);

        cutsceneText.text = "The Offering...";
        yield return new WaitForSeconds(1.5f);

        cutsceneText.text = "The Offering... has begun.";
        yield return new WaitForSeconds(2f);

        // Step 4: Freeze the game to emphasize moment
        Time.timeScale = 0f;
    }
}