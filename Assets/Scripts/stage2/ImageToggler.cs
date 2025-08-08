using UnityEngine;
using UnityEngine.UI;

public class ImageToggler : MonoBehaviour
{
    // Drag your player's Transform into this field in the Inspector.
    public Transform playerTransform;

    // Drag the UI Image you want to show/hide into this field in the Inspector.
    public RawImage displayImage;

    // The maximum distance to the player to allow interaction.
    public float interactionDistance = 0.35f;

    // Internal state to track if the image is currently visible.
    private bool isImageVisible = false;

    void Start()
    {
        // Ensure the image is hidden at the start.
        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player and image are assigned to prevent errors.
        if (playerTransform == null || displayImage == null)
        {
            return;
        }

        // Calculate the distance between this object and the player. [1, 4]
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Check if the player is within interaction distance.
        if (distance <= interactionDistance)
        {
            Debug.Log("Near board");
            // Check if the 'F' key is pressed down. [3, 5]
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Toggle the visibility state.
                isImageVisible = !isImageVisible;

                // Enable or disable the image based on the visibility state. [8, 14]
                displayImage.gameObject.SetActive(isImageVisible);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F) && isImageVisible)
            {
                // Toggle the visibility state.
                isImageVisible = !isImageVisible;

                // Enable or disable the image based on the visibility state. [8, 14]
                displayImage.gameObject.SetActive(isImageVisible);
            }
        }
    }
}