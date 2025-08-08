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
    public NPCDialogue postBossDialogue;
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

    [Header("Audio")]
    public AudioSource backgroundMusic; // assign in inspector
    public float musicFadeDuration = 0.75f;
    public GameObject bossToActivate;

    private void Start()
    {
        dialogueController = DialogueController.Intstance;
        if (fadeGroup != null)
        {
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
        Debug.Log($"NPC {npcId} Interact called. isDialogueActive={isDialogueActive}");
           if (GameManager.Instance.IsWitchsBossDefeated())
            {
                dialogueData = postBossDialogue;
            }
        // Check which dialogue to use based on the save file
        else if (GameManager.Instance.GetNpcFlag(npcId))
        {
            dialogueData = subsequentDialogue;
            Debug.Log($"NPC {npcId}: Using subsequent dialogue.");
        }
        else
        {
            dialogueData = firstTimeDialogue;
            Debug.Log($"NPC {npcId}: Using first time dialogue.");
        }

        if (dialogueData == null)
        {
            Debug.LogWarning($"NPC {npcId} has no dialogue assigned.");
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
        Debug.Log($"NPC {npcId}: Starting dialogue.");
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueController.SetNPCInfo(dialogueData.npcName);
        dialogueController.ShowDialogueUI(true);

        DisplayCurrentLine();
    }

    void Nextline()
    {
        Debug.Log($"NPC {npcId}: Nextline called. Current index: {dialogueIndex}");

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            Debug.Log($"NPC {npcId}: Stopped typing, showing full line.");
            return; // Just finish the line. The state (dialogueIndex) hasn't changed yet.
        }

        dialogueController.clearChoices();

        bool choiceWasFound = false;
        foreach (DialogueChoice choice in dialogueData.choices)
        {
            Debug.Log($"NPC {npcId}: Checking choice for dialogue index: {dialogueIndex} against choice: {choice.dialogueIndex}");
            if (choice.dialogueIndex == dialogueIndex)
            {
                Debug.Log($"NPC {npcId}: Displaying choices for index {dialogueIndex}.");
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
            Debug.Log($"NPC {npcId}: EndDialogue triggered at index {dialogueIndex}.");
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
            Debug.Log($"NPC {npcId}: Dialogue lines exhausted at index {dialogueIndex}, calling EndDialogue.");
            EndDialogue();
        }
    }

    IEnumerator Typeline()
    {
        Debug.Log($"NPC {npcId}: Typing line {dialogueIndex}.");
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
        Debug.Log($"NPC {npcId}: DisplayChoices called for dialogue index {dialogueIndex}.");
        dialogueController.clearChoices();
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndices[i];
            dialogueController.CreateChoiceButton(choice.choices[i], () => ChooseDialogueOption(nextIndex));
        }
    }

    void ChooseDialogueOption(int nextIndex)
    {
        Debug.Log($"NPC {npcId}: ChooseDialogueOption called with nextIndex {nextIndex}.");
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
        Debug.Log($"NPC {npcId}: EndDialogue called.");
        NPCDialogue finishedDialogue = this.dialogueData;
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueController.ShowDialogueUI(false);
        dialogueIndex = 0;
        dialogueController.SetDialogueText("");
        dialogueController.nameText.text = "";
        PauseController.SetPause(false);

        if (npcId == "witch" && dialogueData.triggersCutsceneOnEnd)
        {
            Debug.Log("Witch NPC dialogue ended, triggering cutscene...");
            StartCutsceneAndLoadNextStage();
            return;
        }

        if (finishedDialogue != null && finishedDialogue.triggersBossAppearanceOnEnd)
        {
            
            if (bossToActivate != null)
            {
                bossToActivate.SetActive(true);
                Debug.Log(bossToActivate.name + " has been activated!");
            }
            else
            {
                Debug.LogWarning("Dialogue is set to trigger a boss, but 'Boss To Activate' is not assigned on the NPC.", this.gameObject);
            }
            
            return; 
        }
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
        if (dialogueData.triggersCutsceneOnEnd)
        {
            Debug.Log("Starting cutscene for NPC " + npcId);
            StartCutsceneAndLoadNextStage();
            return; // Important: prevents the stage menu from also opening.
        }
    }

    // ==================== NEW CUTSCENE + FADE + SCENE LOADING CODE ====================

    public void StartCutsceneAndLoadNextStage()
    {
        if (fadeGroup != null && cutsceneVideo != null && !string.IsNullOrEmpty(nextStageName))
        {
            Debug.Log("StartCutsceneAndLoadNextStage called.");
            StartCoroutine(PlayCutsceneAndLoadNextStage());
        }
        else
        {
            PauseController.SetPause(false);
            Debug.LogWarning("Cutscene or fadeGroup or nextStageName missing! Cannot start cutscene sequence for NPC " + npcId);
        }
    }

    private IEnumerator PlayCutsceneAndLoadNextStage()
    {
        Debug.Log("PlayCutsceneAndLoadNextStage coroutine started.");

        // Fade out background music in parallel with fade to black
        if (backgroundMusic != null)
        {
            StartCoroutine(FadeOutAudio(backgroundMusic, musicFadeDuration));
        }

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
        Debug.Log("Loading next scene: " + nextStageName);
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

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}