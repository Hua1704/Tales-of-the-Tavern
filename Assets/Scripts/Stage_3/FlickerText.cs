using UnityEngine;
using TMPro;

public class FlickerText : MonoBehaviour
{
    public GameObject tutorialCanvas;  // Assign your TutorialCanvas here in Inspector
    public float flickerSpeed = 1f;    // Flickers per second

    private TextMeshProUGUI tmpText;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
            Debug.LogWarning("FlickerText: No TextMeshProUGUI found on this GameObject.");
    }

    void Update()
    {
        if (tutorialCanvas != null && tutorialCanvas.activeSelf)
        {
            if (!tmpText.gameObject.activeSelf)
                tmpText.gameObject.SetActive(true);

            // Flicker alpha between 0 and 1
            float alpha = (Mathf.Sin(Time.time * flickerSpeed * Mathf.PI * 2) + 1f) / 2f;
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
        else
        {
            // Hide the text completely when canvas is off
            if (tmpText.gameObject.activeSelf)
                tmpText.gameObject.SetActive(false);
        }
    }
}