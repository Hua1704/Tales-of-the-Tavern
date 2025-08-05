using UnityEngine;

public class StageMenuController : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] private int totalNumberOfStages; // Set this to how many stages your game has
    [SerializeField] private GameObject stageItemPrefab; // The prefab for a single stage row
    [SerializeField] private Transform container; // The parent object for the list items

    // OnEnable is called every time the menu is shown.
    void OnEnable()
    {
        PopulateMenu();
    }

    public void PopulateMenu()
    {
        // 1. Clear any old items from the list to prevent duplicates
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

      
        for (int i = 1; i <= totalNumberOfStages; i++)
        {
            // 3. Create a new item from the prefab
            GameObject itemGO = Instantiate(stageItemPrefab, container);
            StageItemUI itemUI = itemGO.GetComponent<StageItemUI>();

            
            if (itemUI == null)
            {
                Debug.LogError("The 'StageItemPrefab' is missing the 'StageItemUI' script!", stageItemPrefab);
                continue; 
            }

         
            string currentStageName = "Stage" + i;

            bool isCurrentStageComplete = GameManager.Instance.IsStageComplete(currentStageName);
            
          
            bool isLocked = false; 
            if (i > 1) 
            {
                string previousStageName = "Stage" + (i-1);
                isLocked = !GameManager.Instance.IsStageComplete(previousStageName);
            }

           
            itemUI.Setup(i, isCurrentStageComplete, isLocked);
        }
    }
}