using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // Drag your dialogue UI panel into this slot in the inspector
    public GameObject dialoguePanel; 
    
    private bool isPlayerInRange;

    void Start()
    {
        // Make sure the dialogue is hidden when the game starts
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player is in range and presses the E key
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Show the dialogue panel
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
        }
    }

    // This function is called when another collider enters the trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the Player
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered range.");
        }
    }

    // This function is called when another collider exits the trigger zone
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object that exited is the Player
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited range.");

            // Optionally, hide the dialogue if the player walks away
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
}