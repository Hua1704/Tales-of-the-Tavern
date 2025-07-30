using TheProphecy.Enemy;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        BaseEnemy enemyUnit = other.GetComponent<BaseEnemy>();
        if (enemyUnit != null)
        {
            enemyUnit.OnTakeDamage(attackDamage);
        }
    }
}