using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Camera Position")]
    public float height = 5f;           // Height above player
    public float defaultDistance = 6f;   // Default distance behind player (closer than before)
    public float minDistance = 3f;       // Minimum distance when in close quarters
    public float angle = 30f;           // Angle looking down (degrees)
    
    [Header("Close Quarters Detection")]
    public float wallCheckDistance = 8f; // How far to check for walls
    public LayerMask obstacleLayer = ~0; // What layers count as obstacles (default: everything)
    public float distanceAdjustSpeed = 5f; // How quickly camera adjusts distance
    
    [Header("Smoothing")]
    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 2f;  // Lower = smoother, less jerky
    public float directionSmoothSpeed = 3f; // Smoothing for "behind" direction
    
    [Header("Look Target")]
    public Vector3 lookOffset = Vector3.up;  // Offset from player position to look at (e.g., player's head height)
    
    // Store smoothed forward direction to prevent jerky camera movement
    private Vector3 smoothedForwardDirection;
    private float currentDistance; // Current camera distance (smoothly adjusted)

    void Start()
    {
        if (target != null)
        {
            // Initialize smoothed direction to player's current forward
            smoothedForwardDirection = target.forward;
        }
        // Initialize current distance to default
        currentDistance = defaultDistance;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // Try to find player if not set
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                smoothedForwardDirection = target.forward;
            }
            else
                return;
        }

        // Smoothly interpolate the forward direction instead of using it directly
        // This prevents jerky camera movement when player changes direction rapidly
        Vector3 targetForward = target.forward;
        smoothedForwardDirection = Vector3.Slerp(smoothedForwardDirection, targetForward, directionSmoothSpeed * Time.deltaTime);
        
        // Detect close quarters by checking for walls/obstacles behind the player
        float desiredDistance = GetDesiredDistance();
        
        // Smoothly adjust camera distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, distanceAdjustSpeed * Time.deltaTime);
        
        // Calculate camera position behind the player using smoothed direction and dynamic distance
        Vector3 behindPosition = target.position - smoothedForwardDirection * currentDistance;
        
        // Add height offset
        Vector3 desiredPosition = behindPosition + Vector3.up * height;
        
        // Smoothly move camera to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Calculate look target (player position + offset)
        Vector3 lookTarget = target.position + lookOffset;
        
        // Calculate direction from camera to look target
        Vector3 directionToTarget = lookTarget - transform.position;
        
        // Create rotation that looks at the target
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);
        
        // Smoothly rotate camera to look at player
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
    
    float GetDesiredDistance()
    {
        // Start with default distance
        float desiredDist = defaultDistance;
        
        // Check for obstacles/walls in the direction the camera wants to be
        Vector3 checkDirection = -smoothedForwardDirection;
        Vector3 checkOrigin = target.position + Vector3.up * height;
        
        // Cast a ray from player position backwards to see if there's a wall
        RaycastHit hit;
        if (Physics.Raycast(checkOrigin, checkDirection, out hit, wallCheckDistance, obstacleLayer))
        {
            // If we hit something, calculate how close we can get
            float hitDistance = hit.distance;
            
            // Add a small buffer so camera doesn't clip into walls
            float safeDistance = hitDistance - 0.5f;
            
            // Use the closer of: safe distance or minimum distance
            desiredDist = Mathf.Max(minDistance, safeDistance);
            
            // Also check if we're already too close (camera might be inside a wall)
            // In that case, use minimum distance
            if (hitDistance < minDistance)
            {
                desiredDist = minDistance;
            }
        }
        else
        {
            // No obstacles detected, use default distance
            desiredDist = defaultDistance;
        }
        
        // Clamp to valid range
        return Mathf.Clamp(desiredDist, minDistance, defaultDistance);
    }
}

