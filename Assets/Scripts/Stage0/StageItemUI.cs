using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// An enum makes managing states much cleaner than using strings or booleans.
public enum ChapterState { Told, Telling, Untold, Locked }

public class ChapterScrollUI : MonoBehaviour
{
    [SerializeField] private TMP_Text chapterNumberText;
    [SerializeField] private TMP_Text chapterTitleText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button playButton;

    // Colors can be set in the Inspector for easy tweaking.
    [SerializeField] private Color toldColor = Color.green;
    [SerializeField] private Color tellingColor = Color.red;
    [SerializeField] private Color untoldColor = new Color(0.5f, 0.5f, 1f); // A light blue
    [SerializeField] private Color lockedColor = Color.grey;

    public void Setup(ChapterInfo chapterInfo, ChapterState state)
    {
        // Set the static text from the data asset.
        chapterNumberText.text = chapterInfo.chapterNumberText;
        chapterTitleText.text = chapterInfo.chapterTitleText;

        playButton.onClick.RemoveAllListeners();

        // Use a switch to handle the different visual states.
        switch (state)
        {
            case ChapterState.Told:
                statusText.text = "TOLD";
                statusText.color = toldColor;
                playButton.interactable = true; // Allow replaying
                playButton.onClick.AddListener(() => SceneManager.LoadScene(chapterInfo.sceneToLoad));
                break;

            case ChapterState.Telling:
                statusText.text = "TELLING...";
                statusText.color = tellingColor;
                playButton.interactable = true; // Allow continuing
                playButton.onClick.AddListener(() => SceneManager.LoadScene(chapterInfo.sceneToLoad));
                break;

            case ChapterState.Untold:
                statusText.text = "UNTOLD";
                statusText.color = untoldColor;
                playButton.interactable = true;
                playButton.onClick.AddListener(() => {
                    GameManager.Instance.SetCurrentChapter(chapterInfo.chapterId);
                    SceneManager.LoadScene(chapterInfo.sceneToLoad);
                });
                break;

            case ChapterState.Locked:
                chapterTitleText.text = ""; 
                statusText.text = "LOCKED";
                statusText.color = lockedColor;
               
                break;
        }
    }
        public void SetupForComingSoon()
    {
       
        chapterNumberText.text = "";
        chapterTitleText.text = "COMING SOON";

        
        statusText.text = "";
        statusText.color = lockedColor; 

   
       
    }
}