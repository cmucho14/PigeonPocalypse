using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : MonoBehaviour
{
    public void ApplyWaveStats(int waveIndex)
    {
        // waveIndex: 0 = wave1, 1 = wave2, 2 = wave3

        // TEST MODE: Set all enemy HP to 1 for testing
        bool testMode = true; // Set to false to restore normal wave scaling
        
        float healthMult = 1f + 0.5f * waveIndex;   // 1.0, 1.5, 2.0
        float dmgMult = 1f + 0.3f * waveIndex;   // 1.0, 1.3, 1.6
        float speedMult = 1f + 0.15f * waveIndex;  // 1.0, 1.15, 1.30

        Health h = GetComponent<Health>();
        if (h != null)
        {
            if (testMode)
            {
                // Test mode: all enemies have 1 HP
                h.maxHealth = 1f;
            }
            else
            {
                // Normal mode: scale health by wave
                h.maxHealth *= healthMult;
            }
            h.currentHealth = h.maxHealth; // refill to new max
        }

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.attackDamage *= dmgMult;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed *= speedMult;
        }
    }
}
