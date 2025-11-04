using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float waitTime = 0.5f;
    public bool startAtFirstWaypoint = true;
    
    [Header("Waypoints")]
    public List<Transform> waypoints = new List<Transform>();
    
    [Header("Movement Type")]
    public bool isLoop = true;  // If false, platform will reverse at end
    
    private int currentWaypointIndex = 0;
    private int direction = 1;
    private float waitCounter = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool isWaiting = false;
    
    void Start()
    {
        if (waypoints.Count > 0 && startAtFirstWaypoint)
        {
            transform.position = waypoints[0].position;
        }
    }

    void FixedUpdate()
    {
        if (waypoints.Count < 2) return;  // Need at least 2 waypoints to move
        
        if (isWaiting)
        {
            waitCounter += Time.fixedDeltaTime;
            if (waitCounter >= waitTime)
            {
                isWaiting = false;
                waitCounter = 0f;
            }
            return;
        }

        // Get current target waypoint
        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        
        // Move towards target
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPos, 
            ref velocity, 
            0.5f,  // Smoothing time
            moveSpeed
        );

        // Check if we've reached the waypoint
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            isWaiting = true;  // Start waiting at waypoint
            
            // Update waypoint index based on movement type
            if (isLoop)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            }
            else
            {
                if (currentWaypointIndex >= waypoints.Count - 1)
                {
                    direction = -1;
                }
                else if (currentWaypointIndex <= 0)
                {
                    direction = 1;
                }
                currentWaypointIndex += direction;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision is from above (player landing on platform)
        if (collision.contacts[0].normal.y < -0.5f)
        {
            // Make the colliding object (player) a child of the platform
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Remove parent when object leaves platform
        if (collision.transform.parent == transform)
        {
            collision.transform.SetParent(null);
        }
    }

    // Optional: Visualize waypoints and path in editor
    void OnDrawGizmos()
    {
        if (waypoints.Count < 2) return;

        // Draw lines between waypoints
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i+1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
            }
        }

        // If looping, connect last to first
        if (isLoop && waypoints[0] != null && waypoints[waypoints.Count-1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Count-1].position, waypoints[0].position);
        }

        // Draw spheres at waypoints
        Gizmos.color = Color.red;
        foreach (var waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawWireSphere(waypoint.position, 0.3f);
            }
        }
    }
}
