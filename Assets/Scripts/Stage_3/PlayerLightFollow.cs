using UnityEngine;

public class DarknessController : MonoBehaviour
{
    public Transform player;               // Player transform
    public Material darknessMaterial;      // Material using the shader
    [Range(0.1f, 10f)]
    public float lightRadius = 1.5f;      
    [Range(0.01f, 5f)]
    public float smoothEdge = 1f;          

    void Update()
    {
        if (darknessMaterial && player)
        {
            darknessMaterial.SetVector("_PlayerWorldPos", player.position);
            darknessMaterial.SetFloat("_LightRadius", lightRadius);    
            darknessMaterial.SetFloat("_SmoothEdge", smoothEdge);     
        }
    }
}