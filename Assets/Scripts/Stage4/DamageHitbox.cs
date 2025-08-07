using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    [Tooltip("The amount of damage this hitbox will deal.")]
    public int damage = 1;

    private EnemyAtkController enemyController;

    void Awake()
    {
        // Find the main controller on the parent object.
        // This allows any hitbox, no matter how deep, to find its manager.
        enemyController = GetComponentInParent<EnemyAtkController>();
        if (enemyController == null)
        {
            Debug.LogError("DamageHitbox could not find an EnemyAttackController on its parents!", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Boss hit player");
        // Ensure the object is the player and the controller was found
        if (other.CompareTag("Player") && enemyController != null)
        {
            
            // Tell the main controller that we hit something.
            // The controller will decide if damage should be dealt.
            enemyController.RegisterHit(other, damage);
        }
    }
}