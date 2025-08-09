using UnityEngine;
using UnityEngine.SceneManagement;

public class Lost : MonoBehaviour
{
    public void OnPressRetry()
    {
        SceneManager.LoadScene("Stage0");
    }
    public void OnPressMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
