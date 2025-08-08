using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject[] tutorialPages;
    public AudioSource pageSound;

    private int currentPage = 0;
    private bool isVisible = true;

    void Start()
    {
        ShowPage(0);
        PauseGame(isVisible);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isVisible = !isVisible;
            tutorialCanvas.SetActive(isVisible);
            PauseGame(isVisible);
        }

        if (isVisible && Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentPage < tutorialPages.Length - 1)
            {
                currentPage++;
                PlaySound();
                ShowPage(currentPage);
            }
        }

        if (isVisible && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentPage > 0)
            {
                currentPage--;
                PlaySound();
                ShowPage(currentPage);
            }
        }
    }

    void ShowPage(int pageIndex)
    {
        for (int i = 0; i < tutorialPages.Length; i++)
            tutorialPages[i].SetActive(i == pageIndex);
    }

    void PlaySound()
    {
        if (pageSound != null)
            pageSound.Play();
    }

    void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }
}