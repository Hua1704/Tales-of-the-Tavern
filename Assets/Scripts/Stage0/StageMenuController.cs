using UnityEngine;
using System.Collections.Generic;

public class JourneyMenuController : MonoBehaviour
{
    [SerializeField] private Transform container; // The parent object for the scrolls (e.g., a Grid Layout Group)
    [SerializeField] private GameObject chapterScrollPrefab; // The prefab we just made
    [SerializeField] private List<ChapterInfo> chaptersInOrder; // Assign all your ChapterInfo assets here in order.

    void OnEnable()
    {
        PopulateJourney();
    }

    void PopulateJourney()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < chaptersInOrder.Count; i++)
        {
            ChapterInfo currentChapter = chaptersInOrder[i];
            GameObject scrollGO = Instantiate(chapterScrollPrefab, container);
            ChapterScrollUI scrollUI = scrollGO.GetComponent<ChapterScrollUI>();

            // Determine the state of the current chapter
            ChapterState state;
            bool isComplete = GameManager.Instance.IsStageComplete(currentChapter.chapterId);
            bool isCurrent = (GameManager.Instance.GetCurrentChapter() == currentChapter.chapterId);
            
            // The first chapter is never locked. Subsequent chapters are unlocked if the previous one is complete.
            bool isLocked = (i > 0) && !GameManager.Instance.IsStageComplete(chaptersInOrder[i - 1].chapterId);

            if (isLocked)
            {
                state = ChapterState.Locked;
            }
            else if (isComplete)
            {
                state = ChapterState.Told;
            }
            else if (isCurrent)
            {
                state = ChapterState.Telling;
            }
            else
            {
                state = ChapterState.Untold;
            }

            scrollUI.Setup(currentChapter, state);
        }
    }
     public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}