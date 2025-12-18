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
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private Collider[] hitEnemies = new Collider[10]; // Buffer for overlap checks
    private System.Collections.Generic.HashSet<Health> hitThisAttack =
        new System.Collections.Generic.HashSet<Health>();
    [SerializeField] private string enemyLayerName = "Enemy";


    void Start()
    {
        // Auto-set enemy layer mask so you don't need to wire it in Inspector
        enemyLayer = LayerMask.GetMask(enemyLayerName);
        Debug.Log($"[PlayerAttack] enemyLayer mask set to '{enemyLayerName}' = {enemyLayer.value}");

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
        bool wasAttacking = isAttacking;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isAttacking = stateInfo.IsName("Attack") && stateInfo.normalizedTime < 0.95f;

            if (isAttacking)
            {
                // normalizedTime increases beyond 1 if it loops; take fractional part
                float t = stateInfo.normalizedTime % 1f;

                // If time wrapped around, a new swing started -> clear hit list
                if (!wasAttacking || t < lastAttackTime)
                {
                    hitThisAttack.Clear();
                }

                lastAttackTime = t;

                // Only check during the active swing window
                if (t > 0.2f && t < 0.8f)
                {
                    CheckForHit();
                }
            }
            else
            {
                lastAttackTime = 0f;
                hitThisAttack.Clear();
            }
        }
    }


    // Call this from animation event at the peak of the attack swing
    // To add animation event: Select Attack clip → Add Event → Function: OnAttackHit
    public void OnAttackHit()
    {
        CheckForHit();
    }

    void CheckForHit()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (attackPoint == null)
        {
            Debug.LogWarning("[Attack] attackPoint is null.");
            return;
        }

        // Use attackPoint as the hitbox center
        Vector3 center = attackPoint.position;

        // BIGGER buffer so terrain/props don’t crowd out the boss
        // (You can set this at the top too; keeping it here is fine while debugging)
        if (hitEnemies == null || hitEnemies.Length < 64)
            hitEnemies = new Collider[64];

        // Only check enemy layer (your Start() already forces it)
        int hitCount = Physics.OverlapSphereNonAlloc(
            center, attackRadius, hitEnemies, enemyLayer, QueryTriggerInteraction.Collide);

        bool hitAny = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider c = hitEnemies[i];
            if (c == null) continue;

            // Ignore player self
            if (c.transform == transform || c.transform.IsChildOf(transform))
                continue;

            // Find Health on collider OR parent (boss usually has Health on root)
            Health h = c.GetComponentInParent<Health>();
            if (h == null) continue;

            // IMPORTANT: Track by HEALTH, not collider, so boss with multiple colliders works
            // Change your field to HashSet<Health> at the top:
            // private HashSet<Health> hitThisAttack = new HashSet<Health>();
            if (hitThisAttack.Contains(h))
                continue;

            h.TakeDamage(attackDamage);
            Debug.Log($"[Attack] HIT {h.gameObject.name} for {attackDamage}. HP={h.currentHealth}/{h.maxHealth}");

            hitThisAttack.Add(h);
            hitAny = true;
        }

        if (hitAny)
            lastAttackTime = Time.time;
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

