using UnityEngine;

public class BossManger : MonoBehaviour
{   public int deathcount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void onEnemyDeath()
    {
        deathcount += 1;
        if (deathcount == 7)
        {
            GameManager.Instance.SetWitchsBossDefeated(true);

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
