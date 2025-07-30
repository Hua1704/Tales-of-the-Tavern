using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class DialogueController : MonoBehaviour
{
    public static DialogueController Intstance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Intstance == null)
        {
            Intstance = this;
        }
        else Destroy(gameObject);

    }

    // Update is called once per frame
    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);

    }
    public void SetNPCInfo(string npcName, Sprite npcPortrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = npcPortrait;
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
