using UnityEngine;
using TMPro;

public class FlickerText : MonoBehaviour
{
    public GameObject tutorialCanvas;
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

            // Use unscaledTime so flicker continues when timeScale = 0
            float alpha = (Mathf.Sin(Time.unscaledTime * flickerSpeed * Mathf.PI * 2) + 1f) / 2f;
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
        else
        {
            if (tmpText.gameObject.activeSelf)
                tmpText.gameObject.SetActive(false);
        }
    }
}