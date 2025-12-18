using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class WaveSpawnerBox : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Waves (stops after last wave)")]
    public int[] enemiesPerWave = new int[] { 10, 20, 30 };

    [Header("Spawn Timing")]
    public float timeBetweenSpawns = 0.1f;

    [Header("XP Rewards")]
    public int xpPerKill = 5;
    private PlayerXP playerXP;

    [Header("Tree Drop Settings")]
    public float raycastDownDistance = 200f;
    public LayerMask groundLayers = ~0;
    public float navMeshSampleRadius = 3f;
    public float spawnYOffset = 0.1f;

    public System.Action onAllWavesComplete;

    private BoxCollider box;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerXP = playerObj.GetComponent<PlayerXP>();
            if (playerXP == null)
                Debug.LogWarning("WaveSpawnerBox: Player found but missing PlayerXP component (XP won't increase).");
        }
        else
        {
            Debug.LogWarning("WaveSpawnerBox: No object with tag 'Player' found (XP won't increase).");
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("WaveSpawnerBox: enemyPrefab not assigned.");
            enabled = false;
            return;
        }

        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (currentWaveIndex < enemiesPerWave.Length)
        {
            int count = enemiesPerWave[currentWaveIndex];
            Debug.Log($"[WaveSpawner] Wave {currentWaveIndex + 1} starting with {count} enemies.");

            yield return StartCoroutine(SpawnWave(count));

            while (enemiesAlive > 0)
                yield return null;

            Debug.Log($"[WaveSpawner] Wave {currentWaveIndex + 1} cleared.");
            currentWaveIndex++;
        }

        Debug.Log("[WaveSpawner] All waves complete. Ready for boss.");
        onAllWavesComplete?.Invoke();
    }

    IEnumerator SpawnWave(int count)
    {
        int spawned = 0;
        int tries = 0;
        int maxTries = count * 10;

        while (spawned < count && tries < maxTries)
        {
            tries++;

            if (!TryGetSpawnPointOnNavMesh(out Vector3 spawnPos))
                continue;

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
                enemy.transform.position = spawnPos;
                agent.enabled = true;
                agent.Warp(spawnPos);
            }

            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.ApplyWaveStats(currentWaveIndex);
            }

            enemiesAlive++;

            Health h = enemy.GetComponent<Health>();
            if (h != null)
            {
                h.onDeath += () =>
                {
                    enemiesAlive--;

                    // ✅ Enemy death sound
                    if (AudioManager.I != null)
                        AudioManager.I.PlayEnemyDeath();

                    // XP
                    if (playerXP != null)
                        playerXP.AddXP(xpPerKill);
                };
            }
            else
            {
                Debug.LogWarning("WaveSpawnerBox: Enemy missing Health; wave completion may break.");
            }

            spawned++;

            if (timeBetweenSpawns > 0f)
                yield return new WaitForSeconds(timeBetweenSpawns);
        }

        if (spawned < count)
        {
            Debug.LogWarning($"[WaveSpawner] Only spawned {spawned}/{count} enemies (couldn't find enough valid navmesh points).");
        }
    }

    bool TryGetSpawnPointOnNavMesh(out Vector3 navSpawn)
    {
        navSpawn = Vector3.zero;

        Vector3 random = GetRandomPointInBoxWorld();

        if (!Physics.Raycast(random, Vector3.down, out RaycastHit hit, raycastDownDistance, groundLayers, QueryTriggerInteraction.Ignore))
            return false;

        Vector3 groundPoint = hit.point;

        if (!NavMesh.SamplePosition(groundPoint, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas))
            return false;

        navSpawn = navHit.position + Vector3.up * spawnYOffset;
        return true;
    }

    Vector3 GetRandomPointInBoxWorld()
    {
        Bounds b = box.bounds;
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y),
            Random.Range(b.min.z, b.max.z)
        );
    }
}
