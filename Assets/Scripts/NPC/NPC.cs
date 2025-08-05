using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class NPC : MonoBehaviour, IInteractable
{
    public string npcId; 
    public NPCDialogue firstTimeDialogue;
    public NPCDialogue subsequentDialogue;
    private NPCDialogue dialogueData;
    public bool opensStageMenuOnEnd;
    private int dialogueIndex;
    private bool isDialogueActive, isTyping;
    private DialogueController dialogueController;

    public int stageNumber;

    [Header("Cutscene and Transition")]
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.75f;
    public VideoPlayer cutsceneVideo; // assign in inspector
    public string nextStageName; // scene to load after cutscene

    private void Start()
    {   
        dialogueController = DialogueController.Intstance;
        if (fadeGroup != null) {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
        }
          if (string.IsNullOrEmpty(npcId))
    {
        Debug.LogError("This NPC has no ID! Please assign a unique ID in the Inspector.", this.gameObject);
    }
    }

    public bool canInteract()
    {
        return !isDialogueActive;
    }

        public void Interact()
        {
            // Check which dialogue to use based on the save file
            if (GameManager.Instance.GetNpcFlag(npcId))
            {
                dialogueData = subsequentDialogue;
            }
            else
            {
                dialogueData = firstTimeDialogue;
            }

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

        dialogueController.clearChoices();

        bool choiceWasFound = false;
        foreach (DialogueChoice choice in dialogueData.choices)
        {
            Debug.Log("Checking choice for dialogue index: " + dialogueIndex + " against choice: " + choice.dialogueIndex);
            if (choice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(choice);
                choiceWasFound = true;
                break; // Found the choice for this line, stop searching.
            }
        }

        if (choiceWasFound)
        {
            return; // If we displayed choices, wait for player input.
        }

        if (dialogueData.endDialogueLine.Length > dialogueIndex && dialogueData.endDialogueLine[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        dialogueIndex++;

        if (dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator Typeline()
    {
        isTyping = true;
        dialogueController.SetDialogueText("");
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueController.SetDialogueText(dialogueController.dialogueText.text + letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;

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

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(Typeline());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueController.ShowDialogueUI(false);
        dialogueIndex = 0;
        dialogueController.SetDialogueText("");
        dialogueController.nameText.text = "";
        PauseController.SetPause(false);
         if (!GameManager.Instance.GetNpcFlag(npcId))
    {
        GameManager.Instance.SetNpcFlag(npcId, true);
    }
     if (dialogueData.marksStageAsComplete == true)
    {
       GameManager.Instance.MarkStageAsComplete("Stage" + this.stageNumber); 
       Debug.Log("Stage " + this.stageNumber + " has been marked as complete!");
    }


    if (opensStageMenuOnEnd)
    {
        
        dialogueController.ShowPlayStory(true);
    }

    }

    // ==================== NEW CUTSCENE + FADE + SCENE LOADING CODE ====================

    public void StartCutsceneAndLoadNextStage()
    {
        if (fadeGroup != null && cutsceneVideo != null && !string.IsNullOrEmpty(nextStageName))
        {
            StartCoroutine(PlayCutsceneAndLoadNextStage());
        }
        else
        {
            PauseController.SetPause(false);
            Debug.LogWarning("Cutscene or fadeGroup or nextStageName missing! Cannot start cutscene sequence.");
        }
    }

    private IEnumerator PlayCutsceneAndLoadNextStage()
    {
        // Fade to black
        yield return FadeTo(1f, fadeDuration);

        // Prepare video
        cutsceneVideo.Prepare();
        while (!cutsceneVideo.isPrepared)
            yield return null;

        // Play video
        cutsceneVideo.Play();

        // Fade video in (fade UI back to transparent)
        yield return FadeTo(0f, fadeDuration);

        // Wait until video finishes
        while (cutsceneVideo.isPlaying)
            yield return null;

        // Fade back to black
        yield return FadeTo(1f, fadeDuration);

        // Load next scene
        SceneManager.LoadScene(nextStageName);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (!fadeGroup) yield break;
        fadeGroup.blocksRaycasts = true;
        float start = fadeGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        fadeGroup.alpha = targetAlpha;
        fadeGroup.blocksRaycasts = targetAlpha > 0.001f;
    }
}