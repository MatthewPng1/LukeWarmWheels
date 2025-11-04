using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Copy of PlayerMovement that allows full horizontal control while in the air
public class PlayerMovementAir : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpForce = 10f;

    public Animator playerAnimator;

    float horizontalMovement;
    bool isGrounded;
    bool wasGrounded;  // Track previous grounded state for landing detection
    bool isOnPlatform;
    BoxCollider2D playerCollider;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        // If no ground check is assigned, create one
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.5f, 0); // Adjust based on your sprite size
            groundCheck = check.transform;
        }

        // Configure rigidbody settings
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        wasGrounded = isGrounded;  // Store previous grounded state
        // Check if grounded for other logic (landing, drop-through platforms, etc.)
        CheckGrounded();

        // Allow horizontal movement regardless of grounded state (full air control)
        float targetVelocityX = horizontalMovement * speed;

        // Apply movement with minimal velocity threshold
        float newVelocityX = targetVelocityX;

        // If moving very slowly while grounded, allow complete stop (preserve previous behavior)
        if (isGrounded && Mathf.Abs(targetVelocityX) < 0.01f && Mathf.Abs(newVelocityX) < 0.5f)
        {
            newVelocityX = 0f;
        }

        // Flip sprite based on movement direction while preserving the current X scale magnitude
        if (horizontalMovement != 0)
        {
            float currentAbsX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(currentAbsX * Mathf.Sign(horizontalMovement), transform.localScale.y, transform.localScale.z);
        }

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        // --- Animation logic ---
        if (playerAnimator != null)
        {
            // Running animation: true if moving (ignore grounded state)
            if (Mathf.Abs(horizontalMovement) > 0.01f)
            {
                playerAnimator.SetBool("run", true);
            }
            else
            {
                playerAnimator.SetBool("run", false);
            }

            // Update grounded state for jump/fall animations
            playerAnimator.SetBool("isGrounded", isGrounded);
            playerAnimator.SetFloat("yVelocity", rb.linearVelocity.y);
            
            // Detect landing
            if (!wasGrounded && isGrounded)
            {
                // Landing logic can go here if you want to add a landing animation
            }
        }
    }

    private void CheckGrounded()
    {
        // Use a more reliable circle overlap check instead of raycasts
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset grounded/platform state - we'll recompute from overlaps
        isGrounded = false;
        isOnPlatform = false;

        foreach (Collider2D col in colliders)
        {
            if (col == null || col.gameObject == gameObject) continue;

            // Check if we're actually above the collider by comparing positions
            if (groundCheck.position.y > col.bounds.min.y)
            {
                Vector2 closestPoint = col.ClosestPoint(groundCheck.position);
                float verticalDistance = Mathf.Abs(groundCheck.position.y - closestPoint.y);

                // Only consider grounded if we're close enough to the surface
                if (verticalDistance < groundCheckRadius * 0.5f)
                {
                    isGrounded = true;

                    // mark platform if collider is a platform
                    if (col.CompareTag("Platform"))
                    {
                        isOnPlatform = true;
                    }
                }
            }
        }

        // If we're falling fast enough, don't consider grounded to prevent edge sticking
        if (rb.linearVelocity.y < -2f)
        {
            isGrounded = false;
            isOnPlatform = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            // Reset horizontal velocity and then add jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            // Note: do NOT reset horizontalMovement here so player can control direction in air

            // Trigger jump animation
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("jump");
            }
        }
        else if (context.canceled)
        {
            rb.AddForce(new Vector2(0, -jumpForce * 0.5f), ForceMode2D.Impulse);
            playerAnimator.SetTrigger("jump");
        }
    }
    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.5f));
        }
    }
    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
        // Reset velocity if we hit something from the side
        if (!isGrounded && Mathf.Abs(collision.contacts[0].normal.x) > 0.5f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        if(collision.gameObject.CompareTag("killzone"))
        {
            SimpleGameManager.instance.RestartLevel();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
}
