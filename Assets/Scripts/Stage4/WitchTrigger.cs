using UnityEngine;

public class WitchTrigger : MonoBehaviour
{
    public GameObject enemyToActivate;  // Gán enemy ở Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Nhớ set tag Player cho nhân vật
        {
            enemyToActivate.SetActive(true);  // Hiện enemy
            Debug.Log("Enemy xuất hiện!");
        }
    }
}
