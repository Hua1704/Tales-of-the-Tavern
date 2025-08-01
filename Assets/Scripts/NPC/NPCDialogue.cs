using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NPCDialogue", menuName = "NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName; // Name of the NPC
    public Sprite npcPortrait; // Sprite representing the NPC
    public string[] dialogueLines; // Array of dialogue lines for the NPC
    public float typingSpeed = 0.05f; // Speed at which the dialogue is displayed
    public AudioClip[] voiceLines; // Array of audio clips for the NPC's voice lines
    public float voiceVolume = 1.0f; // Volume of the NPC's voice lines
    public bool[] autoProgressLines;
    public float autoProgressLineDelay = 2.0f; // Delay before automatically progressing to the next line
    public DialogueChoice[] choices; // Array of choices available in the dialogue
    public bool[] endDialogueLine;
    [Header("End of Dialogue Action")]
    public bool changeSceneOnEnd;
    public string nextSceneName;
    public bool triggerPlayStoryOnEnd;
    public bool openTabOnEnd; 
    public int tabIndexToOpen;

}
[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndices; // Index of the dialogue line that this choice corresponds to
}
