using UnityEngine;
using System.Collections;

public class ConversationPlayer : MonoBehaviour
{
    public static ConversationPlayer Instance { get; private set; }

    private DialogueController dialogueController;
    private ConversationData currentConversation;
    private int lineIndex;

    private bool isConversationActive = false;
    private bool isTyping = false;
    private System.Action onConversationEnded;

    // --- THIS IS THE FIX ---
    // We use Awake() to get references. It runs before any other script's Start() or OnTriggerEnter().
    // This guarantees 'dialogueController' will NOT be null when StartConversation is called.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        
        // Get the reference to our UI controller immediately.
        dialogueController = DialogueController.Intstance;
    }

    // Start() is no longer needed for initialization.

    private void Update()
    {
        // Use a more specific input if you have one, otherwise this is fine.
        if (isConversationActive && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                // We subtract 1 from lineIndex because it was already incremented for the *next* line.
                dialogueController.SetDialogueText(currentConversation.conversationLines[lineIndex - 1].line);
                isTyping = false;
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    public void StartConversation(ConversationData conversation, System.Action onEndCallback)
    {
        if (conversation == null || conversation.conversationLines.Length == 0) return;

        // This safety check is now more robust because dialogueController is guaranteed to be set.
        if (dialogueController == null)
        {
            Debug.LogError("DialogueController instance not found! Make sure it's in the scene.");
            return;
        }

        currentConversation = conversation;
        onConversationEnded = onEndCallback;
        lineIndex = 0;
        isConversationActive = true;
        
        dialogueController.ShowDialogueUI(true);
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (lineIndex >= currentConversation.conversationLines.Length)
        {
            EndConversation();
            return;
        }

        DialogueLine currentLine = currentConversation.conversationLines[lineIndex];
        
        dialogueController.SetNPCInfo(currentLine.speaker.npcName);
        StartCoroutine(TypeLine(currentLine.line));
        
        lineIndex++;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueController.SetDialogueText("");
        float typingSpeed = 0.05f; 
        foreach (char letter in line.ToCharArray())
        {
            dialogueController.SetDialogueText(dialogueController.dialogueText.text + letter);
            // Use unscaledDeltaTime if you plan to pause the game during these conversations.
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

   private void EndConversation()
{
    isConversationActive = false;
    dialogueController.ShowDialogueUI(false);

    if (currentConversation != null && currentConversation.marksStageAsComplete)
    {
        
        if (!string.IsNullOrEmpty(currentConversation.stageNameToComplete))
        {
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.MarkStageAsComplete(currentConversation.stageNameToComplete);
                Debug.Log("Stage '" + currentConversation.stageNameToComplete + "' has been marked as complete by a conversation.");
            }
            else
            {
                Debug.LogError("GameManager.Instance is not found! Cannot mark stage as complete.");
            }
        }
        else
        {
            Debug.LogWarning("Conversation is set to mark a stage as complete, but 'Stage Name To Complete' is empty.", currentConversation);
        }
    }

    onConversationEnded?.Invoke();
}
}