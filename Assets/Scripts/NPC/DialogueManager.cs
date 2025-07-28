using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;  // Reference to the UI Text for displaying dialogue
    public Button[] choiceButtons;  // Array of buttons for player choices

    private bool isInRange = false;  // Check if the player is in range of the NPC
    private bool isDialogueActive = false;
    private int currentLine = 0;

    // Dialogue and choices setup
    private Dialogue[] dialogues;

    void Start()
    {
        dialogues = new Dialogue[]
        {
            new Dialogue("Hello, adventurer! How can I help you?", new string[] { "Tell me about the town.", "I'm just passing by." }),
            new Dialogue("The town is peaceful, but watch out for the forest. Some strange things are happening.", new string[] { "What do you mean?", "Thanks, I’ll be careful." }),
            new Dialogue("There's been a lot of rumors about a monster in the woods.", new string[] { "I’ll go check it out!", "I’m not interested in monsters." })
        };

        // Hide the choice buttons initially
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the player presses the 'E' key to interact with the NPC
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            StartDialogue();
        }
    }

    // When the player enters the NPC's range
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    // When the player leaves the NPC's range
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    // Start the dialogue
    void StartDialogue()
    {
        isDialogueActive = true;
        currentLine = 0;
        ShowDialogue(dialogues[currentLine]);
    }

    // Show the dialogue and choices
    void ShowDialogue(Dialogue dialogue)
    {
        dialogueText.text = dialogue.text;

        // Show the choice buttons
        for (int i = 0; i < dialogue.choices.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            int choiceIndex = i;  // Local copy for the button callback
            choiceButtons[i].GetComponentInChildren<Text>().text = dialogue.choices[i];
            choiceButtons[i].onClick.RemoveAllListeners(); // Clear any previous listeners
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
    }

    // Handle the player's choice
    void OnChoiceSelected(int choiceIndex)
    {
        currentLine++;
        if (currentLine < dialogues.Length)
        {
            ShowDialogue(dialogues[currentLine]);
        }
        else
        {
            EndDialogue();
        }

        // Hide the choice buttons after selection
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    // End the dialogue
    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueText.text = "";  // Clear the dialogue box
    }
}

// Dialogue class to hold each dialogue and its choices
[System.Serializable]
public class Dialogue
{
    public string text;
    public string[] choices;

    public Dialogue(string text, string[] choices)
    {
        this.text = text;
        this.choices = choices;
    }
}
