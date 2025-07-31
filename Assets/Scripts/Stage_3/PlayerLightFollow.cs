using UnityEngine;

public class DarknessControllerTwoLights : MonoBehaviour
{
    public Transform player;
    public Transform startLight;           // Fixed light at start
    public Material darknessMaterial;

    [Header("Player Light")]
    [Range(0.1f, 10f)] public float playerRadius = 1.5f;

    [Header("Start Light")]
    [Range(0.1f, 10f)] public float startRadius = 2f;

    [Header("Edge Smoothness")]
    [Range(0.01f, 5f)] public float smoothEdge = 1f;

    void Update()
    {
        if (darknessMaterial)
        {
            if (player)
                darknessMaterial.SetVector("_PlayerWorldPos", player.position);
            if (startLight)
                darknessMaterial.SetVector("_StartWorldPos", startLight.position);

            darknessMaterial.SetFloat("_LightRadius", playerRadius);
            darknessMaterial.SetFloat("_StartRadius", startRadius);
            darknessMaterial.SetFloat("_SmoothEdge", smoothEdge);
        }
    }
}