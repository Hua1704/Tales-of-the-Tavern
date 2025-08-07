using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
      private void OnDisable()
    {
        
        playerControls.Movement.Disable();
    }
    private void Update()
    {
        PlayerInput();
    }   
    
    private void FixedUpdate()
    {
        Move();
    }
    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
        myAnimator.SetFloat("speed", movement.sqrMagnitude); 

       
        if (movement.sqrMagnitude > 0.01f)
        {
            myAnimator.SetFloat("lastX", movement.x);
            myAnimator.SetFloat("lastY", movement.y);
        }

    }
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }   
   
}
