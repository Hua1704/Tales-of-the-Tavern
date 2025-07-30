using UnityEngine;
using System.Collections;

public class PlayerControllerStage2 : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float slideSpeed = 5f;

    [Header("Combat")]
    [SerializeField] private GameObject attackUp;
    [SerializeField] private GameObject attackDown;
    [SerializeField] private GameObject attackLeft;
    [SerializeField] private GameObject attackRight;

    [Header("Collision Settings")]
    [Tooltip("Set this to the layer that contains your walls, trees, and other obstacles.")]
    [SerializeField] private LayerMask obstacleLayer;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private Collider2D playerCollider; // Reference to the player's own collider

    // State management variables
    private bool isOnMud = false;
    private Vector2 slideDirection;
    private float lastAttackTime=0f;
    private float attackCooldown = 0.35f;
    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>(); // Get our own collider
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if (isOnMud)
        {
            HandleMudLogic();
        }
        else
        {
            HandleNormalMovement();
            HandleAttack();
        }
    }

    private void FixedUpdate()
    {
        if (isOnMud)
        {
            rb.MovePosition(rb.position + slideDirection * (slideSpeed * Time.fixedDeltaTime));
        }
        else
        {
            rb.MovePosition(rb.position + movement.normalized * (moveSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleMudLogic()
    {
        // Check for new input ONLY if we are currently stopped
        if (slideDirection == Vector2.zero && movement.sqrMagnitude > 0.01f)
        {
            movement = Mathf.Abs(movement.x) > Mathf.Abs(movement.y) ? new Vector2(Mathf.Sign(movement.x), 0f) : new Vector2(0f, Mathf.Sign(movement.y));

            // Get the potential direction from input
            Vector2 potentialDirection = movement.normalized;

            // --- THE FIX: Check if the path is blocked before starting a slide ---
            RaycastHit2D hit = Physics2D.BoxCast(
                playerCollider.bounds.center, // The center of our player's collider
                playerCollider.bounds.size,   // The size of our collider (slightly smaller to avoid self-collision)
                0f,                           // Angle of the box
                potentialDirection,           // The direction to check
                0.1f,                         // A very short distance to check for an immediate wall
                obstacleLayer                 // Only check for collisions on the obstacle layer
            );

            // If the BoxCast did NOT hit anything, it's safe to start sliding.
            if (hit.collider == null)
            {
                slideDirection = potentialDirection;
                myAnimator.SetFloat("lastX", slideDirection.x);
                myAnimator.SetFloat("lastY", slideDirection.y);
            }
            // If the BoxCast hit a collider, do nothing. The player remains stopped
            // and is free to input a different, unblocked direction.
        }

        // Update animator to reflect sliding motion or idle state on mud
        myAnimator.SetFloat("moveX", slideDirection.x);
        myAnimator.SetFloat("moveY", slideDirection.y);
        myAnimator.SetFloat("speed", slideDirection.sqrMagnitude);
    }

    private void HandleNormalMovement()
    {
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
        myAnimator.SetFloat("speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            myAnimator.SetFloat("lastX", movement.x);
            myAnimator.SetFloat("lastY", movement.y);
        }
    }
    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            myAnimator.SetTrigger("attack");
            lastAttackTime = Time.time;
        }
    }

    // Called by Animation Event at the start of the attack swing
    public void EnableAttackCollider()
    {
        float lastX = myAnimator.GetFloat("lastX");
        float lastY = myAnimator.GetFloat("lastY");

        if (Mathf.Abs(lastX) > Mathf.Abs(lastY))
        {
            if (lastX > 0) attackRight.SetActive(true);
            else attackLeft.SetActive(true);
        }
        else
        {
            if (lastY > 0) attackUp.SetActive(true);
            else attackDown.SetActive(true);
        }
    }
    public void DisableAttackColliders()
    {
        attackUp.SetActive(false);
        attackDown.SetActive(false);
        attackLeft.SetActive(false);
        attackRight.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mud"))
        {
            isOnMud = true;
            // Stop normal movement immediately upon entering mud
            movement = Vector2.zero;
            // Let the player choose their first slide direction from a standstill
            slideDirection = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Mud"))
        {
            isOnMud = false;
            slideDirection = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isOnMud)
        {
            slideDirection = Vector2.zero;
        }
    }
}