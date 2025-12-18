using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Sword Setup")]
    public GameObject swordPrefab;
    public Transform handBone; // Assign the hand bone (e.g., Hand_R, RightHand, etc.)
    public string handBoneName = "hand.r"; // Fallback: will search for this bone name
    
    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;
    
    [Header("Attack Detection")]
    public Transform attackPoint; // Optional: specific attack point (sword tip, etc.)
    public float attackRadius = 1.5f;
    
    private GameObject swordInstance;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false;
    private Collider[] hitEnemies = new Collider[10]; // Buffer for overlap checks
    private System.Collections.Generic.HashSet<Collider> hitThisAttack = new System.Collections.Generic.HashSet<Collider>();
    
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        
        // If Animator is on a different GameObject, add a helper component there to forward animation events
        if (animator != null && animator.gameObject != gameObject)
        {
            // Add a helper component to the Animator's GameObject to forward events
            AnimationEventForwarder forwarder = animator.gameObject.GetComponent<AnimationEventForwarder>();
            if (forwarder == null)
            {
                forwarder = animator.gameObject.AddComponent<AnimationEventForwarder>();
            }
            forwarder.playerAttack = this;
            Debug.Log($"PlayerAttack: Added AnimationEventForwarder to {animator.gameObject.name} to handle animation events.");
        }
        
        // Warn if enemy layer is not set
        if (enemyLayer == 0)
        {
            Debug.LogWarning("PlayerAttack: enemyLayer is not set! Attack will check all colliders. Please set the enemy layer in the Inspector for better performance.");
        }
        
        // Find hand bone if not assigned
        if (handBone == null)
        {
            handBone = FindBoneByName(handBoneName);
            if (handBone == null)
            {
                Debug.LogWarning($"PlayerAttack: Could not find bone '{handBoneName}'. Please assign handBone manually in Inspector.");
            }
        }
        
        // Spawn and attach sword
        if (swordPrefab != null && handBone != null)
        {
            swordInstance = Instantiate(swordPrefab, handBone);
            swordInstance.transform.localPosition = Vector3.zero;
            swordInstance.transform.localRotation = Quaternion.identity;
            
            // Make all colliders on the sword triggers so they don't physically block enemies
            Collider[] swordColliders = swordInstance.GetComponentsInChildren<Collider>();
            foreach (Collider col in swordColliders)
            {
                col.isTrigger = true;
            }
            
            // Set attack point to sword tip if available, otherwise use hand position
            if (attackPoint == null)
            {
                // Try to find a child object named "Tip" or create an empty at sword end
                Transform tip = swordInstance.transform.Find("Tip");
                if (tip == null)
                {
                    GameObject tipObj = new GameObject("AttackPoint");
                    tipObj.transform.SetParent(swordInstance.transform);
                    tipObj.transform.localPosition = new Vector3(0, 0, 1f); // Adjust based on sword length
                    attackPoint = tipObj.transform;
                }
                else
                {
                    attackPoint = tip;
                }
            }
        }
        else if (swordPrefab == null)
        {
            Debug.LogWarning("PlayerAttack: No sword prefab assigned!");
        }
    }
    
    void Update()
    {
        // Check if attack animation is playing
        bool wasAttacking = isAttacking;
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isAttacking = stateInfo.IsName("Attack") && stateInfo.normalizedTime < 0.9f;
        }
        
        // Continuously check for hits during attack animation (in addition to animation event)
        // This ensures hits are detected even if animation event isn't set up
        if (isAttacking && !wasAttacking)
        {
            // Just started attacking, clear previous hits
            hitThisAttack.Clear();
        }
        
        // Check for hits continuously during the active part of the attack animation
        if (isAttacking && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // Check during the middle portion of the attack (when sword is swinging)
            if (stateInfo.normalizedTime > 0.2f && stateInfo.normalizedTime < 0.8f)
            {
                CheckForHit();
            }
        }
        
        // Clear hit list when attack ends
        if (wasAttacking && !isAttacking)
        {
            hitThisAttack.Clear();
        }
    }
    
    // Call this from animation event at the peak of the attack swing
    // To add animation event: Select Attack clip → Add Event → Function: OnAttackHit
    public void OnAttackHit()
    {
        if (AudioManager.I != null)
        AudioManager.I.PlaySlash();
        CheckForHit();
    }
    
    void CheckForHit()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;
        
        Vector3 checkPosition = attackPoint != null ? attackPoint.position : transform.position + transform.forward * attackRange;
        
        // If enemyLayer is not set (all zeros), check all colliders (fallback)
        int hitCount;
        if (enemyLayer == 0)
        {
            // Fallback: check all colliders if layer mask isn't set
            hitCount = Physics.OverlapSphereNonAlloc(checkPosition, attackRadius, hitEnemies);
        }
        else
        {
            // Find all enemies in range using layer mask
            hitCount = Physics.OverlapSphereNonAlloc(checkPosition, attackRadius, hitEnemies, enemyLayer);
        }
        
        bool hitAnyEnemy = false;
        for (int i = 0; i < hitCount; i++)
        {
            Collider enemyCollider = hitEnemies[i];
            
            // Skip if null or already hit in this attack
            if (enemyCollider == null || hitThisAttack.Contains(enemyCollider))
                continue;
            
            // Skip if this is the player's own collider
            if (enemyCollider.transform == transform || enemyCollider.transform.IsChildOf(transform))
                continue;
            
            // Try to get Health component from the collider's GameObject or its parent
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth == null)
            {
                // Try parent in case collider is on a child object
                enemyHealth = enemyCollider.GetComponentInParent<Health>();
            }
            
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                hitThisAttack.Add(enemyCollider); // Mark as hit
                hitAnyEnemy = true;
                Debug.Log($"Player hit {enemyCollider.name} for {attackDamage} damage! Enemy HP: {enemyHealth.currentHealth}/{enemyHealth.maxHealth}");
                
                // Optional: Add knockback or visual feedback here
            }
        }
        
        if (hitAnyEnemy)
        {
            lastAttackTime = Time.time;
        }
    }
    
    Transform FindBoneByName(string boneName)
    {
        // Search in all children recursively
        return FindBoneRecursive(transform, boneName);
    }
    
    Transform FindBoneRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name))
                return child;
            
            Transform found = FindBoneRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    
    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRadius);
        }
    }
}

