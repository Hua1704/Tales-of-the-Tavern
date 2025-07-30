using TheProphecy.Map.PathFinding;
using UnityEngine;
using TheProphecy.Map.DungeonGeneration;
using System.Collections;


namespace TheProphecy.Enemy
{
    public class EnemyMovementAI : MonoBehaviour
    {
        [Header("References")]
        private SpriteRenderer _spriteRenderer;
        private AccessReferencesForAI _accessReferencesForAI;

        private Transform _gridGameObject;
        private PathfindingGrid _grid;
        private Animator _animator;
        private Pathfinding _pathfinding;

        private Transform _targetLeft; // choosing 2 target points fixes bug of wrong calculation of nodes!
        private Transform _targetRight;

        InvisibilityController _invisibilityController;

        [Header("Pathfinding")]
        private Vector3[] _waypoints;
        private Vector3 _oldTargetPosition;

        private const float _PATH_UPDATE_TIME = 0.07f;
        private float _pathUpdateTimer = 0f;

        private int _currentCheckPointIndex = 0;
        private float _moveSpeed = 0.35f;

        [Header("Combat")]
        [SerializeField] private int attackDamage = 1;
        private float _range = 1f;
        private float _attackRange = 0.2f;
        private bool _isInRange = false;

        private float lastAttackTime;
        private float attackCooldown = 1.5f;

        private BaseUnit _playerBaseUnit; // Reference to the player's health script

        void Start()
        {

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _accessReferencesForAI = transform.GetComponentInParent<AccessReferencesForAI>();

            _gridGameObject = _accessReferencesForAI.pathfindingGrid.transform;
            _targetLeft = _accessReferencesForAI.targetLeftPivot.transform;
            _targetRight = _accessReferencesForAI.targetRightPivot.transform;

            _pathfinding = _gridGameObject.GetComponent<Pathfinding>();
            _grid = _pathfinding.Grid;
            _oldTargetPosition = _targetLeft.position;
            _range = 1.0f;
            _animator = GetComponent<Animator>();
            _waypoints = _pathfinding.FindPath(transform.position, _targetLeft.position);
            lastAttackTime = 0.0f;
            // Find the player and get their BaseUnit component
            _playerBaseUnit = _targetLeft.GetComponent<BaseUnit>();
        }

        private void Update()
        {

            _isInRange = _range > (transform.position - _targetLeft.position).magnitude;
            bool _isInAttackRange = _attackRange > (transform.position - _targetLeft.position).magnitude;
            if (_isInRange)
            {
                UpdatePath();
            }
            if(_isInAttackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                _animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }

        }

        private void FixedUpdate()
        {
            if (_isInRange )
            {
                FollowPath();
            }
            else
            {
                _animator.SetFloat("speed", 0);
            }
        }
        public void TriggerAttack()
        {
            // Check if the player is in attack range at the moment the animation hits.
            float distanceToPlayer = (transform.position - _targetLeft.position).magnitude;

            // You can add a small buffer to the range to be more forgiving.
            if (distanceToPlayer <= _attackRange + 0.1f)
            {
                if (_playerBaseUnit != null)
                {
                    _playerBaseUnit.OnTakeDamage(attackDamage);
                }
            }
        }
        private Vector3 ChooseTargetPivotOfCharacter()
        {
            Node targetNodeLeft = _grid.NodeFromWorldPoint(_targetLeft.position);

            if (targetNodeLeft.walkable)
            {
                return _targetLeft.position;
            }

            return _targetRight.position;

        }

        private void UpdatePath()
        {
            Vector3 target = ChooseTargetPivotOfCharacter();

            if (_pathUpdateTimer < _PATH_UPDATE_TIME)
            {
                _pathUpdateTimer += Time.deltaTime;
            }

            else
            {
                _pathUpdateTimer = 0f;

                
                Node targetNode = _grid.NodeFromWorldPoint(target);
                Node oldTargetNode = _grid.NodeFromWorldPoint(_oldTargetPosition);

                if (!(targetNode.Equals(oldTargetNode)))
                {
                    _waypoints = _pathfinding.FindPath(transform.position, target);
                    _oldTargetPosition = target;
                    _currentCheckPointIndex = 0;
                }
            }
        }

        private void FollowPath()
        {
            if (_currentCheckPointIndex < _waypoints.Length)
            {
                PathfindingGrid grid = _pathfinding.Grid;
                Node currentTransformNode = grid.NodeFromWorldPoint(transform.position);
                Node nextWaypointNode = grid.NodeFromWorldPoint(_waypoints[_currentCheckPointIndex]);

                // if currentTransformNode.Equals(nextWaypointNode)
                if ((nextWaypointNode.worldPosition - transform.position).magnitude < 0.15f)
                {
                    _currentCheckPointIndex++;
                }

                if (_currentCheckPointIndex < _waypoints.Length)
                {
                    Vector3 moveDirection = (_waypoints[_currentCheckPointIndex] - transform.position).normalized;
                    transform.Translate(moveDirection * Time.deltaTime * _moveSpeed);
                    // --- Set Animator Parameters Here ---
                    // Pass the x and y components of the moveDirection to the Animator
                    _animator.SetFloat("DirectionX", moveDirection.x);
                    _animator.SetFloat("DirectionY", moveDirection.y);
                    _animator.SetFloat("speed", _moveSpeed);
                }

            }
        }

        public void OnDrawGizmos()
        {
            if (_waypoints != null)
            {
                for (int i = 0; i < _waypoints.Length; i++)
                {
                    Vector3 pos = _waypoints[i];
                    if (i < _currentCheckPointIndex)
                    {
                        Gizmos.color = Color.green;
                    }

                    else if (i == _currentCheckPointIndex)
                    {
                        Gizmos.color = Color.yellow;
                    }

                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawCube(pos, Vector3.one * (0.45f));
                }
            }
        }
    }

    
}
