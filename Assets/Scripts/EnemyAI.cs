using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private NavMeshAgent agent;
    private float lastAttackTime;

    private EnemyPeckAnim peckAnim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        peckAnim = GetComponentInChildren<EnemyPeckAnim>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            
            // Ignore collisions with player so enemies don't push the player
            IgnorePlayerCollisions(playerObj);
        }
        else
        {
            Debug.LogWarning("EnemyAI: No object with tag 'Player' found. AI will idle until one exists.");
        }
    }
    
    void IgnorePlayerCollisions(GameObject playerObj)
    {
        Collider[] enemyColliders = GetComponentsInChildren<Collider>();
        Collider[] playerColliders = playerObj.GetComponentsInChildren<Collider>();
        
        foreach (Collider enemyCol in enemyColliders)
        {
            foreach (Collider playerCol in playerColliders)
            {
                if (enemyCol != null && playerCol != null && enemyCol != playerCol)
                {
                    Physics.IgnoreCollision(enemyCol, playerCol, true);
                }
            }
        }
    }

    private void Update()
    {
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        // Play hop + peck animation
        if (peckAnim != null)
            peckAnim.Play();

        // Deal damage
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy hit player for {attackDamage}. Player HP now: {playerHealth.currentHealth}");
        }
        else
        {
            Debug.LogWarning("EnemyAI: Player has no Health Component to damage.");
        }

    }
    //revert

}
