using UnityEngine;

public class CameraFollows : MonoBehaviour
{
    public Transform target;    // Drag the Player here
    public Vector3 offset;      // Adjust offset in Inspector
    public float smoothSpeed = 5f;  // Adjust how smooth the movement is

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothed.x, smoothed.y, transform.position.z); // Keep camera Z
    }
}