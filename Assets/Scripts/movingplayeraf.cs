using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Wheelchair Movement Settings")]
    public float acceleration = 12f;    // reduced acceleration for wheelchair
    public float maxSpeed = 5f;         // lower top speed for wheelchair
    public float brakingForce = 20f;    // moderate braking for stability
    public float baseSteeringSpeed = 90f;  // base rotation speed when stationary
    public float maxSteeringSpeed = 120f;  // maximum rotation speed when moving
    [Range(0.2f, 1f)] public float minTurnRadius = 0.5f;  // minimum turning radius factor

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Wheelchair movement controls
        bool forwardPressed = Input.GetKey(KeyCode.UpArrow);
        bool reversePressed = Input.GetKey(KeyCode.DownArrow);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);

        float forwardInput = 0f;
        if (forwardPressed) forwardInput = 1f;
        else if (reversePressed) forwardInput = -1f;

        float steerInput = 0f;
        if (leftPressed) steerInput = 1f;
        else if (rightPressed) steerInput = -1f;

        // Apply movement force only in facing direction
        float currentSpeed = rb.linearVelocity.magnitude;
        if (forwardInput != 0f)
        {
            if (currentSpeed < maxSpeed)
            {
                rb.AddForce(transform.up * forwardInput * acceleration);
            }
        }
        else
        {
            // Natural rolling friction
            rb.linearVelocity *= 0.98f;
        }

        // Speed limit
        if (currentSpeed > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Steering always allowed
        float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
        float effectiveSteeringSpeed = Mathf.Lerp(baseSteeringSpeed, maxSteeringSpeed, speedFactor);
        float turnRestriction = Mathf.Lerp(1f, minTurnRadius, speedFactor);
        float rotation = steerInput * effectiveSteeringSpeed * turnRestriction * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);

        // Only allow movement in facing direction (no lateral drift)
        Vector2 forwardVel = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        rb.linearVelocity = forwardVel;
    }
}
