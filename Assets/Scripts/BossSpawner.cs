using UnityEngine;
using UnityEngine.AI;

public class BossSpawner : MonoBehaviour
{
    [Header("References")]
    public WaveSpawnerBox waveSpawner;     // drag your EnemySpawner object here
    public GameObject bossPrefab; 
    private AudioManager audioManager;
         // your boss prefab

    [Header("Spawn")]
    public Transform spawnPoint;           // optional (can leave null)
    public float navMeshSampleRadius = 5f;

    private bool spawned;

    void Awake()
    {
        if (waveSpawner == null)
            waveSpawner = FindObjectOfType<WaveSpawnerBox>();

            audioManager = FindObjectOfType<AudioManager>();
    }

    void OnEnable()
    {
        if (waveSpawner != null)
            waveSpawner.onAllWavesComplete += SpawnBoss;
    }

    void OnDisable()
    {
        if (waveSpawner != null)
            waveSpawner.onAllWavesComplete -= SpawnBoss;
    }

    void SpawnBoss()
    {
        if (spawned) return;
        spawned = true;

        if (bossPrefab == null)
        {
            Debug.LogError("BossSpawner: bossPrefab not assigned.");
            return;
        }

        Vector3 desired = spawnPoint != null ? spawnPoint.position : transform.position;

        // snap to navmesh so the boss always moves
        if (NavMesh.SamplePosition(desired, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            desired = hit.position;

        GameObject boss = Instantiate(bossPrefab, desired, Quaternion.identity);

        // force agent placement if present
        var agent = boss.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
            boss.transform.position = desired;
            agent.enabled = true;
            agent.Warp(desired);
        }

        // Ensure boss doesn't block player - set up collision ignoring
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            BossAI bossAI = boss.GetComponent<BossAI>();
            if (bossAI != null)
            {
                // Call the collision ignoring method if it's public, or do it here
                Collider[] bossColliders = boss.GetComponentsInChildren<Collider>();
                Collider[] playerColliders = playerObj.GetComponentsInChildren<Collider>();
                
                foreach (Collider bossCol in bossColliders)
                {
                    foreach (Collider playerCol in playerColliders)
                    {
                        if (bossCol != null && playerCol != null && bossCol != playerCol)
                        {
                            Physics.IgnoreCollision(bossCol, playerCol, true);
                        }
                    }
                }
            }
        }

        Debug.Log("[BossSpawner] Boss spawned!");
    }
}
