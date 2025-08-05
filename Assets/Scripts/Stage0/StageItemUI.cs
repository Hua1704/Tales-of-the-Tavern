using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button playButton;

   
    public void Setup(int stageNumber, bool isCompleted, bool isLocked)
    {
       
        stageNameText.text = "Stage " + stageNumber; 
        statusText.text = isCompleted ? "COMPLETED" : "INCOMPLETE";
        statusText.color = isCompleted ? Color.green : Color.yellow;

        playButton.interactable = !isLocked;

        
        if (isLocked)
        {
            playButton.GetComponentInChildren<TMP_Text>().text = "Locked";
        }

      
        playButton.onClick.RemoveAllListeners();
        
        playButton.onClick.AddListener(() => {
            
            SceneManager.LoadScene("Stage" + stageNumber);
        });
    }
}