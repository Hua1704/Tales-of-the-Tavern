using UnityEngine;
using TheProphecy.Interfaces;
using UnityEngine.UI;
using TheProphecy.LevelRun;
using Random = UnityEngine.Random;

namespace TheProphecy.Enemy
{
    public class BaseEnemy : BaseUnit, IDamageable
    {
        [SerializeField] private Transform playerCoord;
        [Header("Drop Variables")]
        [SerializeField] private int _minCoinDropRate = 0;
        [SerializeField] private int _maxCoinDropRate = 5;
        [SerializeField] private GameObject _dropitem;
        protected override void Die()
        {
            base.Die();
            gameObject.SetActive(false);
            // Direction from player to enemy (opposite of toward player)
            Vector3 directionAwayFromPlayer = (transform.position - playerCoord.position).normalized;
            float dropOffsetDistance = 0.1f; // You can adjust this value
            Vector3 dropPosition = transform.position + directionAwayFromPlayer * dropOffsetDistance;

            Instantiate(_dropitem, dropPosition, transform.rotation);

            // LevelRunStats levelStats = LevelManager.instance.levelRunStats;
            // levelStats.AddKill();
            // levelStats.AddCoins(Random.Range(_minCoinDropRate, _maxCoinDropRate));
        }
    }
}
