using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class DialogueController : MonoBehaviour
{
    public static DialogueController Intstance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public GameObject playStoryPanel;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Intstance == null)
        {
            Intstance = this;
        }
        else Destroy(gameObject);
        if (playStoryPanel != null) playStoryPanel.SetActive(false);
        
        // This is from your previous request, it should stay.
           

    }
   
      public void ShowPlayStory(bool show)
    {
        if (playStoryPanel != null)
        {
            playStoryPanel.SetActive(show);
        }   
        else
        {
            Debug.LogError("Play Story Panel has not been assigned in the DialogueController Inspector!");
        }
    }

    // Update is called once per frame
    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);

    }
    public void SetNPCInfo(string npcName)
    {
        nameText.text = npcName;
       
    }
    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }
    public void clearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }
    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClickAction)
    {   
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClickAction);
       
    }
   
}
