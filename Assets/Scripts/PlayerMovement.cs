using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float rotationSpeed = 10f;
    
    [Header("Ground Check")]
    public float groundCheckRadius = 0.3f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Animation")]
    public Animator animator;
    public string speedParam = "Speed";
    public string directionXParam = "DirectionX";
    public string directionYParam = "DirectionY";
    public string attackTrigger = "Attack";
    
    private Rigidbody rb;
    private bool isGrounded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Lock rotation so physics doesn't spin the player
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add a collider if missing
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            var col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 0.5f;
            col.center = Vector3.up;
            playerCollider = col;
        }
        
        // Note: Collision ignoring with enemies is handled in EnemyAI.Start() and BossAI.Start()
        // when enemies spawn, so we don't need to do it here
        
        // Try to find Animator on this object or children if not assigned
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    
    void Start()
    {
        // Ensure animator is found and enabled
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        // Initialize animator parameters to zero on start to ensure animator is ready
        if (animator != null)
        {
            // Force animator to update immediately
            animator.enabled = true;
            animator.Update(0f); // Force an update to ensure it's ready
            animator.SetFloat(speedParam, 0f);
            animator.SetFloat(directionXParam, 0f);
            animator.SetFloat(directionYParam, 0f);
        }
        else
        {
            Debug.LogWarning("PlayerMovement: Animator not found! Animation will not work.");
        }
    }
    
    // Call this method when new enemies are spawned to ignore collisions with them
    public void IgnoreCollisionWithEnemy(GameObject enemy)
    {
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null && enemy != null)
        {
            Collider[] enemyColliders = enemy.GetComponentsInChildren<Collider>();
            foreach (Collider enemyCol in enemyColliders)
            {
                if (enemyCol != null)
                {
                    Physics.IgnoreCollision(playerCollider, enemyCol, true);
                }
            }
        }
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Ground check using SphereCast for reliability
        isGrounded = CheckGrounded();
        
        // Movement direction
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Update animator parameters FIRST, before movement/rotation
        // This ensures animation responds immediately to input
        if (animator != null && animator.enabled)
        {
            // Calculate speed from input directly (immediate response)
            float inputSpeed = movement.magnitude > 0.1f ? moveSpeed : 0f;
            animator.SetFloat(speedParam, inputSpeed);
            
            // Calculate direction relative to player's CURRENT forward direction
            // Use the player's current rotation (before we rotate it)
            Vector3 localMovement = transform.InverseTransformDirection(movement);
            
            // For 2D Blend Tree: X = left/right (-1 to 1), Y = forward/backward (-1 to 1)
            animator.SetFloat(directionXParam, localMovement.x);
            animator.SetFloat(directionYParam, localMovement.z);
        }
        
        // Apply movement
        Vector3 velocity = movement * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        
        // Rotate player to face movement direction
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        
        // Attack input (left mouse button)
        if (Input.GetMouseButtonDown(0) && animator != null && animator.enabled)
        {
            animator.SetTrigger(attackTrigger);
        }
    }
    
    bool CheckGrounded()
    {
        // Cast a sphere downward to check for ground
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        
        if (groundLayer != 0)
        {
            return Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.5f, groundLayer);
        }
        else
        {
            // If no ground layer set, check against everything
            return Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.5f);
        }
    }
}
