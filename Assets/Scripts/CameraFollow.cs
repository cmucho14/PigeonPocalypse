using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Camera Position")]
    public float height = 5f;           // Height above player
    public float distance = 10f;        // Distance behind player
    public float angle = 30f;           // Angle looking down (degrees)
    
    [Header("Smoothing")]
    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 2f;  // Lower = smoother, less jerky
    public float directionSmoothSpeed = 3f; // Smoothing for "behind" direction
    
    [Header("Look Target")]
    public Vector3 lookOffset = Vector3.up;  // Offset from player position to look at (e.g., player's head height)
    
    // Store smoothed forward direction to prevent jerky camera movement
    private Vector3 smoothedForwardDirection;

    void Start()
    {
        if (target != null)
        {
            // Initialize smoothed direction to player's current forward
            smoothedForwardDirection = target.forward;
        }
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
        
        // Calculate camera position behind the player using smoothed direction
        Vector3 behindPosition = target.position - smoothedForwardDirection * distance;
        
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
}

