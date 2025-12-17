using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    [Header("Chase")]
    public float attackRange = 2.5f;

    [Header("Melee Attack")]
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    [Header("Boss Slam (AOE)")]
    public float slamRange = 5f;
    public float slamDamage = 15f;
    public float slamCooldown = 6f;

    [Header("Enrage Phase")]
    [Range(0.1f, 0.9f)]
    public float enrageAtHpPercent = 0.5f;     // enrages at 50%
    public float enragedSpeedMultiplier = 1.35f;
    public float enragedDamageMultiplier = 1.4f;
    public float enragedCooldownMultiplier = 0.75f;

    private Transform player;
    private NavMeshAgent agent;
    private Health bossHealth;
    private BeakSlamAnim beakAnim;

    private float lastMeleeTime;
    private float lastSlamTime;

    private bool enraged;
    private float baseSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;

        bossHealth = GetComponent<Health>();
        beakAnim = GetComponentInChildren<BeakSlamAnim>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogWarning("BossAI: No object with tag 'Player' found.");

        // Optional: make the boss feel “heavier”
        agent.stoppingDistance = Mathf.Max(agent.stoppingDistance, 1.2f);
    }

    void Update()
    {
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        HandleEnrage();

        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        // Prefer slam if available and in range
        if (dist <= slamRange && Time.time - lastSlamTime >= slamCooldown)
        {
            DoSlam();
            return;
        }

        // Otherwise melee if close enough
        if (dist <= attackRange)
        {
            TryMelee();
        }
    }

    void HandleEnrage()
    {
        if (enraged) return;
        if (bossHealth == null) return;

        float hpPct = bossHealth.currentHealth / bossHealth.maxHealth;
        if (hpPct <= enrageAtHpPercent)
        {
            enraged = true;

            agent.speed = baseSpeed * enragedSpeedMultiplier;
            attackDamage *= enragedDamageMultiplier;
            slamDamage *= enragedDamageMultiplier;

            attackCooldown *= enragedCooldownMultiplier;
            slamCooldown *= enragedCooldownMultiplier;

            Debug.Log("[BossAI] ENRAGED!");
        }
    }

    void TryMelee()
    {
        if (Time.time - lastMeleeTime < attackCooldown) return;
        lastMeleeTime = Time.time;

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Boss melee hit for {attackDamage}. Player HP: {playerHealth.currentHealth}");
        }
    }

    void DoSlam()
    {
        lastSlamTime = Time.time;

        // Face the player before the slam (feels way better)
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(toPlayer);

        // Play slam animation (new method name)
        if (beakAnim != null)
            beakAnim.PlaySlam();

        // Damage (AOE simplified as "within range" check)
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= slamRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(slamDamage);
            }
        }
    }
}
