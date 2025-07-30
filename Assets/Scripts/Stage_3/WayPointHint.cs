using UnityEngine;
using System.Collections;

public class SoundWaypoint : MonoBehaviour
{
    public enum WaypointType { Correct, Wrong }
    public WaypointType type;

    public AudioSource roarSource;
    public float totalCorrect = 15f;
    public float totalWrong = 13f;

    private static float correctHits = 0f;
    private static float wrongHits = 0f;

    private bool used = false;
    private Collider2D col;
    private SpriteRenderer sprite;
    
    void Awake()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;

        // Track progress
        if (type == WaypointType.Correct)
        {
            correctHits++;
        }
        else if (type == WaypointType.Wrong)
        {
            wrongHits++;
        }

        // Normalize volume: based on how much closer player is to correct end
        float progress = Mathf.Clamp((correctHits - wrongHits) / totalCorrect, 0.05f, 1f);
        roarSource.volume = progress;

        //Debug.Log("Correct: " + correctHits + ", Wrong: " + wrongHits + ", Volume: " + roarSource.volume);

        // Mark this waypoint as used
        used = true;
        col.enabled = false;
        if (sprite != null) sprite.enabled = false;

        StartCoroutine(ResetWaypoint());
    }

    IEnumerator ResetWaypoint()
    {
        yield return new WaitForSeconds(15f); // Respawn time
        used = false;
        col.enabled = true;
        if (sprite != null) sprite.enabled = true;

        // Don't decrement hits, just reset the trigger itself
    }
}