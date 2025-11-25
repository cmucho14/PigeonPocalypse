using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    
    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    
    private Rigidbody rb;
    private Transform model;
    private Vector3 baseScale;
    private bool isGrounded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        // Find the visual model (mesh renderer child)
        var mr = GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            model = mr.transform;
            baseScale = model.localScale;
        }
        else
        {
            model = transform;
            baseScale = transform.localScale;
        }
        
        // Add a collider if missing
        if (GetComponent<Collider>() == null)
        {
            var col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 0.5f;
            col.center = Vector3.up;
        }
    }
    
    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Check if grounded
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
        
        // If no ground layer set, use simple check
        if (groundLayer == 0)
        {
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f);
        }
        
        // Movement
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        
        // Flip sprite based on direction
        if (horizontal != 0)
        {
            FlipModel(horizontal > 0);
        }
    }
    
    void FlipModel(bool facingRight)
    {
        if (model == null) return;
        
        float sign = facingRight ? 1f : -1f;
        model.localScale = new Vector3(
            Mathf.Abs(baseScale.x) * sign,
            baseScale.y,
            baseScale.z
        );
    }
}

