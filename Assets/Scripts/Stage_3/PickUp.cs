using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightBlessingPickup : MonoBehaviour
{
    [Header("Light Blessing")]
    public DarknessControllerTwoLights darknessController; // your darkness controller
    public float grantedRadius = 1.5f;                     // player light radius after pickup

    [Header("Audio")]
    public AudioSource pickupAudio; // drag a scene AudioSource here
    [Range(0f, 1f)] public float volume = 0.2f;

    private bool _taken;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_taken || !other.CompareTag("Player")) return;
        _taken = true;

        // grant the light blessing
        if (darknessController) darknessController.EnablePlayerLight(grantedRadius);

        // play sound in the simplest way
        if (pickupAudio)
        {
            pickupAudio.volume = volume;
            pickupAudio.Play();
        }

        // hide this pickup and remove it
        var sr = GetComponent<SpriteRenderer>(); if (sr) sr.enabled = false;
        var col = GetComponent<Collider2D>(); if (col) col.enabled = false;

        Destroy(gameObject); // AudioSource lives elsewhere, so it's safe to destroy now
    }
}