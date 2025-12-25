using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    
    private int dialogueIndex;
    private bool isDialogueActive, isTyping;
    private DialogueController dialogueController;
 
    public int stageNumber; 
    private void Start()
    {
        dialogueController = DialogueController.Intstance;
        
    }
    public bool canInteract()
    {
        return !isDialogueActive;
    }
    public void Interact()
    {
        if (dialogueData == null)
        {
            return;
        }
        if (isDialogueActive)
        {
            Nextline();
        }
        else
        {
            StartDialogue();
        }
    }
    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueController.SetNPCInfo(dialogueData.npcName);
        dialogueController.ShowDialogueUI(true);
        PauseController.SetPause(true);
        DisplayCurrentLine();

    }
void Nextline()
{
    if (isTyping)
    {
        StopAllCoroutines();
        dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
        isTyping = false;
        return; // Just finish the line. The state (dialogueIndex) hasn't changed yet.
    }

    // We are now at a "decision point" for the line at the CURRENT dialogueIndex.
    dialogueController.clearChoices();

    // Check if there are choices associated with the line we JUST finished displaying.
    bool choiceWasFound = false;
    foreach (DialogueChoice choice in dialogueData.choices)
    {   Debug.Log("Checking choice for dialogue index: " + dialogueIndex + " against choice: " + choice.dialogueIndex);
        // Does the choice's index match our CURRENT dialogue line index?
        if (choice.dialogueIndex == dialogueIndex)
        {   
            DisplayChoices(choice);
            choiceWasFound = true;
            break; // Found the choice for this line, stop searching.
        }
    }

    if (choiceWasFound)
    {
        return; // If we displayed choices, our job is done. Wait for the player to click a choice button.
    }

    // If no choice was found for the current line, check if this is an end-dialogue line.
    if (dialogueData.endDialogueLine.Length > dialogueIndex && dialogueData.endDialogueLine[dialogueIndex])
    {
        EndDialogue();
        return;
    }

    // If it's not a choice and not an end line, it's time to move to the next line.
    // NOW we increment the index.
    dialogueIndex++;

    // Now that the index is updated, check if we're out of dialogue lines.
    if (dialogueIndex < dialogueData.dialogueLines.Length)
    {
        DisplayCurrentLine(); // This will display the NEW line at the new index.
    }
    else
    {
        // We've run out of lines naturally.
        EndDialogue();
    }
}
    IEnumerator Typeline()
{
    isTyping = true;
    dialogueController.SetDialogueText("");
    foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
    {
        dialogueController.SetDialogueText(dialogueController.dialogueText.text += letter);
        yield return new WaitForSeconds(dialogueData.typingSpeed);
    }
    isTyping = false;
    
    // Auto-progress logic should now call Nextline() to use the central logic
    if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
    {
        yield return new WaitForSeconds(dialogueData.autoProgressLineDelay);
        Nextline();
    }
}
    
    void DisplayChoices(DialogueChoice choice)
    {
        dialogueController.clearChoices();
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndices[i];
            dialogueController.CreateChoiceButton(choice.choices[i], () => ChooseDialogueOption(nextIndex));
        }
    }
    void ChooseDialogueOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueController.clearChoices();
        DisplayCurrentLine();
    }
    void DisplayCurrentLine() {
        StopAllCoroutines();
        StartCoroutine(Typeline());
    }
  public void EndDialogue()
{
    // Standard cleanup
    StopAllCoroutines();
    isDialogueActive = false;
    dialogueController.ShowDialogueUI(false);
    dialogueIndex = 0;
    dialogueController.SetDialogueText("");
    dialogueController.nameText.text = "";

    
   
    // ACTION 1: Open a specific tab in a menu?
    if (dialogueData.openTabOnEnd)
    {
        // Call the new public method on our TabController singleton.
        TabController.Instance.OpenMenuAndActivateTab(dialogueData.tabIndexToOpen);
        // The game remains paused while the menu is open.
    }
     
    else if (dialogueData.changeSceneOnEnd)
    {
        if (!string.IsNullOrEmpty(dialogueData.nextSceneName))
        {
            SceneManager.LoadScene(dialogueData.nextSceneName);
        }
    }
    // DEFAULT ACTION: If nothing else, just unpause the game.
    else
    {
        PauseController.SetPause(false);
    }
}
    


}
