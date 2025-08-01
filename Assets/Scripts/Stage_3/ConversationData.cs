using UnityEngine;


[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
public class ConversationData : ScriptableObject
{
   
    public DialogueLine[] conversationLines;
}


[System.Serializable]
public class DialogueLine
{
    
    public NPCDialogue speaker;

    [TextArea(3, 10)]
    public string line;
}