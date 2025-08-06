using TheProphecy.Enemy;
using TheProphecy.Interfaces;
using UnityEngine;
/*
public class BossEnemy : BaseUnit, IDamageable
{
    [SerializeField] private Transform playerCoord;

    [Header("Spawning Variables")]
    [Tooltip("The prefab of this boss, used for spawning smaller versions.")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Current generation of this boss. 1 = Full, 2 = Half, 3 = Quarter.")]
    public int spawnGeneration = 1;

    [Header("Drop Variables")]
    [SerializeField] private int _minCoinDropRate = 0;
    [SerializeField] private int _maxCoinDropRate = 5;
    [SerializeField] private GameObject _dropitem;

    protected override void Die()
    {
        base.Die();
        // The isAlive flag is already set to false in BaseUnit's OnTakeDamage
        gameObject.SetActive(false);
        // If this is not the final generation, spawn children
        if (spawnGeneration < 3)
        {
            SpawnChildren();
        }

        // Deactivate the current boss after handling its death logic
        
    }

    private void SpawnChildren()
    {
        // Spawn two smaller bosses
        for (int i = 0; i < 2; i++)
        {
            // Spawn them slightly to the left and right of the parent's position
            Vector3 spawnOffset = (i == 0) ? new Vector3(-0.2f, 0, 0) : new Vector3(0.2f, 0, 0);
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject childObject = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);

            // --- Configure the new child boss ---
            BossEnemy childBoss = childObject.GetComponent<BossEnemy>();
            if (childBoss != null)
            {
                // Set the new, smaller scale
                childObject.transform.localScale = transform.localScale / 2f;

                // Calculate and set the new health (ensure it's at least 1)
                int newMaxHealth = Mathf.Max(1, MAX_HEALTH / 2);
                childBoss.InitializeUnit(newMaxHealth, newMaxHealth);

                // Increment the generation for the child
                childBoss.spawnGeneration = this.spawnGeneration + 1;

                // Pass down the reference to the player
                childBoss.playerCoord = this.playerCoord;
            }
        }
    }

    private void DropLoot()
    {
        if (_dropitem == null) return;

        // Direction from player to enemy (opposite of toward player)
        Vector3 directionAwayFromPlayer = (transform.position - playerCoord.position).normalized;
        float dropOffsetDistance = 0.1f; // You can adjust this value
        Vector3 dropPosition = transform.position + directionAwayFromPlayer * dropOffsetDistance;

        Instantiate(_dropitem, dropPosition, transform.rotation);

        // Optional: Add stats for the kill
        // LevelRunStats levelStats = LevelManager.instance.levelRunStats;
        // levelStats.AddKill();
        // levelStats.AddCoins(Random.Range(_minCoinDropRate, _maxCoinDropRate));
    }
}
*/


public class BaseEnemy1 : BaseUnit, IDamageable
{
    [Header("Spawning Variables")]
    [Tooltip("The prefab of this boss, used for spawning smaller versions.")]
    [SerializeField] private GameObject bossPrefab;
    [Tooltip("Current generation of this boss. 1 = Full, 2 = Half, 3 = Quarter.")]
    public int spawnGeneration = 1;

    [SerializeField] private Transform playerCoord;
    [Header("Drop Variables")]
    [SerializeField] private int _minCoinDropRate = 0;
    [SerializeField] private int _maxCoinDropRate = 5;
    [SerializeField] private GameObject _dropitem;
    protected override void Die()
    {
        base.Die();
        spriteRenderer.material = originalMaterial;
        // gameObject.SetActive(false);
        // Direction from player to enemy (opposite of toward player)
        //Vector3 directionAwayFromPlayer = (transform.position - playerCoord.position).normalized;
        //float dropOffsetDistance = 0.1f; // You can adjust this value
        //Vector3 dropPosition = transform.position + directionAwayFromPlayer * dropOffsetDistance;

        //Instantiate(_dropitem, dropPosition, transform.rotation);
        // The isAlive flag is already set to false in BaseUnit's OnTakeDamage

        // If this is not the final generation, spawn children
        if (spawnGeneration < 3)
        {
            SpawnChildren();
        }

        // Deactivate the current boss after handling its death logic
        gameObject.SetActive(false);

        // LevelRunStats levelStats = LevelManager.instance.levelRunStats;
        // levelStats.AddKill();
        // levelStats.AddCoins(Random.Range(_minCoinDropRate, _maxCoinDropRate));
    }
    private void SpawnChildren()
    {
        // Spawn two smaller bosses
        for (int i = 0; i < 2; i++)
        {
            // Spawn them slightly to the left and right of the parent's position
            Vector3 spawnOffset = (i == 0) ? new Vector3(-0.2f, 0, 0) : new Vector3(0.2f, 0, 0);
            Vector3 spawnPosition = transform.position + spawnOffset;

            // --- FIX FOR LOCATION AND PARENTING ---
            // Instantiate the new boss at the same position and under the same parent.
            GameObject childObject = Instantiate(bossPrefab, transform.position, Quaternion.identity, transform.parent);
            childObject.SetActive(true);
            // --- Configure the new child boss ---
            BaseEnemy1 childBoss = childObject.GetComponent<BaseEnemy1>();
            if (childBoss != null)
            {
                // Set the new, smaller scale
                childObject.transform.localScale = transform.localScale / 2f;

                // Calculate and set the new health (ensure it's at least 1)
                int newMaxHealth = Mathf.Max(1, MAX_HEALTH / 2);
                childBoss.InitializeUnit(newMaxHealth, newMaxHealth);

                // Increment the generation for the child
                childBoss.spawnGeneration = this.spawnGeneration + 1;

                // Pass down the reference to the player
                childBoss.playerCoord = this.playerCoord;
            }
        }
    }
}
