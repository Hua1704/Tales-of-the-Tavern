using UnityEngine;

public class DarknessControllerTwoLights : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;       // moving light (player)
    public Transform startLight;   // fixed light (start point)
    public Material darknessMaterial;

    [Header("Player Light (enabled after pickup)")]
    public bool playerLightEnabled = false;     // start OFF
    [Range(0.1f, 10f)] public float playerRadius = 1.5f;

    [Header("Start Light (always on)")]
    [Range(0.1f, 10f)] public float startRadius = 2.0f;

    [Header("Edge Smoothness")]
    [Range(0.01f, 5f)] public float smoothEdge = 1f;

    // Call this from the pickup when the player touches it
    public void EnablePlayerLight(float newRadius = -1f)
    {
        playerLightEnabled = true;
        if (newRadius > 0f) playerRadius = newRadius;
    }

    void Update()
    {
        if (!darknessMaterial) return;

        // Positions (world space)
        if (player) darknessMaterial.SetVector("_PlayerWorldPos", player.position);
        if (startLight) darknessMaterial.SetVector("_StartWorldPos", startLight.position);

        // Radii
        if (playerLightEnabled)
        {
            darknessMaterial.SetFloat("_LightRadius", playerRadius);
        }
        else
        {
            // Disable player hole by pushing the radius far negative
            darknessMaterial.SetFloat("_LightRadius", -1000f);
        }

        darknessMaterial.SetFloat("_StartRadius", startRadius);
        darknessMaterial.SetFloat("_SmoothEdge", smoothEdge);
    }
}