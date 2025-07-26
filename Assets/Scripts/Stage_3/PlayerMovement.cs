using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;

    void Update()
    {
        // Default: no movement
        movement = Vector2.zero;

        // Reset all directional animation flags
        animator.SetBool("GoUp", false);
        animator.SetBool("GoDown", false);
        animator.SetBool("GoLeft", false);
        animator.SetBool("GoRight", false);

        // Movement keys
        if (Input.GetKey(KeyCode.W))
        {
            movement = Vector2.up;
            animator.SetBool("GoUp", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement = Vector2.down;
            animator.SetBool("GoDown", true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = Vector2.left;
            animator.SetBool("GoLeft", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement = Vector2.right;
            animator.SetBool("GoRight", true);
        }

        // Optionally, set a bool to trigger idle animation if no key is pressed
        bool isMoving = movement != Vector2.zero;

        // OPTIONAL: Reset animator fully when stopping (clean animation reset)
        if (!isMoving)
        {
            animator.Rebind();   // Resets to default
            animator.Update(0f); // Applies immediately
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}