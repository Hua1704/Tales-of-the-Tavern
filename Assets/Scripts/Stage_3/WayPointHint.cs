using UnityEngine;
using System.Collections;

public class SoundWaypoint : MonoBehaviour
{
    public enum WaypointType { Correct, Wrong }
    public WaypointType type;
    public float volumeStep = 0.2f;
    public AudioSource roarSource;
    private bool used = false;

    private Collider2D col;
    private SpriteRenderer sprite; // optional for visual hiding

    void Awake()
    {
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>(); // optional
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player"))
            return;

        if (type == WaypointType.Correct)
        {
            roarSource.volume = Mathf.Clamp(roarSource.volume + volumeStep, 0f, 1f);
            //Debug.Log("Correct waypoint hit! Volume increased to: " + roarSource.volume);
        }
        else if (type == WaypointType.Wrong)
        {
            roarSource.volume = Mathf.Clamp(roarSource.volume - volumeStep, 0f, 1f);
            //Debug.Log("Wrong waypoint hit! Volume decreased to: " + roarSource.volume);
        }

        used = true;
        col.enabled = false;
        if (sprite != null) sprite.enabled = false;

        StartCoroutine(ResetWaypoint());
    }

    IEnumerator ResetWaypoint()
    {
        yield return new WaitForSeconds(30f); // 30-secs

        used = false;
        col.enabled = true;
        if (sprite != null) sprite.enabled = true;
    }
}