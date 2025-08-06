using System.Collections.Generic;
using TheProphecy.Map.PathFinding;
using UnityEngine;
using System.Collections;
public class BossBehavior : MonoBehaviour
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
    [SerializeField] private float _normalMoveSpeed = 0.35f;
    private float _moveSpeed;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 1;
    private float _range = 1f;
    private float _attackRange = 1.5f;
    private bool _isInRange = false;

    private float lastAttackTime;
    private float attackCooldown = 3f;

    private BaseUnit _playerBaseUnit; // Reference to the player's health script

    // New variables for attack management
    private List<string> _attackAnimations = new List<string> { "attack1", "attack2", "attack3" };
    private bool _isAttacking = false;
    private bool _isCharging = false; // Flag to control behavior during attack2 charge

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
        _range = 10.0f;
        _animator = GetComponent<Animator>();
        _waypoints = _pathfinding.FindPath(transform.position, _targetLeft.position);
        lastAttackTime = 0.0f;
        _moveSpeed = _normalMoveSpeed;

        // Find the player and get their BaseUnit component
        _playerBaseUnit = _targetLeft.GetComponent<BaseUnit>();
    }

    private void Update()
    {
        if (_isCharging) return; // If charging for attack2, skip normal updates

        _isInRange = _range > (transform.position - _targetLeft.position).magnitude;
        bool _isInAttackRange = _attackRange > (transform.position - _targetLeft.position).magnitude;

        if (_isInRange && !_isAttacking)
        {
            UpdatePath();
        }

        if (_isInAttackRange && Time.time - lastAttackTime >= attackCooldown && !_isAttacking)
        {
            ChooseAndExecuteAttack();
        }
    }

    private void FixedUpdate()
    {
        if (_isCharging)
        {
            // Special movement logic for attack2 charge
            Vector3 moveDirection = (_targetLeft.position - transform.position).normalized;
            transform.Translate(moveDirection * Time.deltaTime * _moveSpeed);
            _animator.SetFloat("DirectionX", moveDirection.x);
            _animator.SetFloat("DirectionY", moveDirection.y);
            _animator.SetFloat("speed", _moveSpeed);
            return;
        }

        if (_isInRange && !_isAttacking)
        {
            FollowPath();
        }
        else if (!_isAttacking)
        {
            _animator.SetFloat("speed", 0);
        }
    }

    private void ChooseAndExecuteAttack()
    {
        _isAttacking = true;
        lastAttackTime = Time.time;

        // Randomly select an attack
        int attackIndex = Random.Range(0, _attackAnimations.Count);
        string selectedAttack = _attackAnimations[attackIndex];

        if (selectedAttack == "attack2")
        {
            StartCoroutine(Attack2_ChargeAndStrike());
        }
        else // For attack1 and attack3
        {
            _animator.SetTrigger(selectedAttack);
            // A small coroutine to reset the attacking flag after the animation
            StartCoroutine(ResetAttackFlag(1.0f)); // Assuming attack1/3 animations are about 1s long
        }
    }

    private IEnumerator Attack2_ChargeAndStrike()
    {
        _isCharging = true;
        _moveSpeed = _normalMoveSpeed * 4; // Increase speed

        // 1 second charge towards the player
        yield return new WaitForSeconds(1.0f);

        _isCharging = false;
        _animator.SetTrigger("attack2");

        // Continue with high speed during the animation (assuming animation is ~0.5s long)
        float animationDuration = 0.5f;
        float timer = 0;
        while (timer < animationDuration)
        {
            Vector3 moveDirection = (_targetLeft.position - transform.position).normalized;
            transform.Translate(moveDirection * Time.deltaTime * _moveSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        // Reset everything after the attack
        _moveSpeed = _normalMoveSpeed;
        _isAttacking = false;
        _animator.SetFloat("speed", 0); // Stop moving animation
    }

    private IEnumerator ResetAttackFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isAttacking = false;
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
                //_playerBaseUnit.OnTakeDamage(attackDamage);
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
        // Do not update path if we are in an attack sequence
        if (_isAttacking || _isCharging) return;

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
        if (_waypoints != null && _currentCheckPointIndex < _waypoints.Length)
        {
            PathfindingGrid grid = _pathfinding.Grid;
            Node nextWaypointNode = grid.NodeFromWorldPoint(_waypoints[_currentCheckPointIndex]);

            if ((nextWaypointNode.worldPosition - transform.position).magnitude < 0.15f)
            {
                _currentCheckPointIndex++;
            }

            if (_currentCheckPointIndex < _waypoints.Length)
            {
                Vector3 moveDirection = (_waypoints[_currentCheckPointIndex] - transform.position).normalized;
                transform.Translate(moveDirection * Time.deltaTime * _moveSpeed);
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