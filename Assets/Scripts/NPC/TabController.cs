using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public static TabController Instance { get; private set; }

    // This reference is crucial. It points to the panel we want to activate.
    public GameObject menuPanel; 

    public Image[] tabImages;
    public GameObject[] pages;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OpenMenuAndActivateTab(int tabIndex)
    {
        // Because this script is on the ALWAYS ACTIVE UIManager, this code will now run successfully.
        if (menuPanel != null)
        {
            menuPanel.SetActive(true); // This makes the whole menu visible.
        }

        ActivateTab(tabIndex); // This sets the correct page and tab colors.
    }

    public void ActivateTab(int tabnum)
    {
        // Your existing logic is perfect.
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(false);
            if (tabImages[i] != null) tabImages[i].color = Color.grey;
        }
        if (pages[tabnum] != null) pages[tabnum].SetActive(true);
        if (tabImages[tabnum] != null) tabImages[tabnum].color = Color.white;
    }
}