using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StatsUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] statsSlots;
    public CanvasGroup StatsUICanvas;

    private void Start()
    {
        UpdateAllStats();
        StatsUICanvas.alpha = 0;
        StatsUICanvas.interactable = false;
        StatsUICanvas.blocksRaycasts = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdateAllStats();
            bool isShowing = StatsUICanvas.alpha == 1;

            // Toggle visibility
            StatsUICanvas.alpha = isShowing ? 0 : 1;
            StatsUICanvas.interactable = !isShowing;
            StatsUICanvas.blocksRaycasts = !isShowing;

            // Freeze or resume game
            Time.timeScale = isShowing ? 1f : 0f;
        }
    }

    public void UpdateHP()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "HP:" + StatsManager.Instance.currentHealth;
    }

    public void UpdateATK()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "ATK:" + StatsManager.Instance.attack;
    }

    public void UpdateSPEED()
    {
        statsSlots[2].GetComponentInChildren<TMP_Text>().text = "SPEED:" + StatsManager.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateHP();
        UpdateATK();
        UpdateSPEED();
    }
}
