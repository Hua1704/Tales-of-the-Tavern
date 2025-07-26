using UnityEngine;

public class SoundWaypoint : MonoBehaviour
{
    // ENUM AND VARIABLES DECLARED HERE
    public enum WaypointType { Correct, Wrong }
    public WaypointType type;
    public float volumeStep = 0.2f; // Optional: tweak per waypoint
    public AudioSource roarSource;
    private bool used = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
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
        }
    }
}