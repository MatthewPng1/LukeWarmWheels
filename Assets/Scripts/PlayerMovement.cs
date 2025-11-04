using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpForce = 10f;

    float horizontalMovement;
    bool isGrounded;
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
        // Check if grounded with multiple raycasts
        CheckGrounded();

        // Only allow horizontal movement when grounded
        float targetVelocityX = 0f;
        if (isGrounded)
        {
            targetVelocityX = horizontalMovement * speed;
        }
        else
        {
            // Keep current horizontal velocity while in air
            targetVelocityX = rb.linearVelocity.x;
        }
        
        // Apply movement with minimal velocity threshold
        float newVelocityX = targetVelocityX;
        
        // If moving very slowly while grounded, allow complete stop
        if (isGrounded && Mathf.Abs(targetVelocityX) < 0.01f && Mathf.Abs(newVelocityX) < 0.5f)
        {
            newVelocityX = 0f;
        }
        
        // Flip sprite based on movement direction while preserving the current X scale magnitude
        if (horizontalMovement != 0)
        {
            // Keep the absolute X scale (e.g. 2) and only change its sign to flip
            float currentAbsX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(currentAbsX * Mathf.Sign(horizontalMovement), transform.localScale.y, transform.localScale.z);
        }
        
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
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
            // Ignore our own collider
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

                    // If any of the colliders under us is tagged Platform, mark isOnPlatform
                    if (col.CompareTag("Platform"))
                    {
                        isOnPlatform = true;
                    }

                    // don't break early; allow detection of platform colliders even if a non-platform
                    // collider was found first (helps when multiple colliders overlap)
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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            // Reset horizontal movement input to prevent continued movement
            horizontalMovement = 0;
        }
        else if (context.canceled)
        {
            rb.AddForce(new Vector2(0, -jumpForce * 0.5f), ForceMode2D.Impulse);
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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
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
}

