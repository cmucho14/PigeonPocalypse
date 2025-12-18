using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;

    public System.Action onDeath;
    public System.Action<float, float> onHealthChanged; // current, max

    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"[ENEMY HEALTH] {gameObject.name}: {currentHealth}/{maxHealth}");

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[ENEMY DEAD] {gameObject.name} died");

        EnemyXPReward reward = GetComponent<EnemyXPReward>();
        if (reward != null)
        {
            Debug.Log($"[XP DROP] {gameObject.name} dropping {reward.xpOnDeath} XP");

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerXP xp = playerObj.GetComponent<PlayerXP>();
                if (xp != null)
                {
                    xp.AddXP(reward.xpOnDeath);
                }
                else
                {
                    Debug.LogWarning("[XP ERROR] Player has no PlayerXP component");
                }
            }
            else
            {
                Debug.LogWarning("[XP ERROR] No Player object found");
            }
        }
        else
        {
            Debug.LogWarning("[XP WARNING] Enemy has no EnemyXPReward component");
        }

        onDeath?.Invoke();
        Destroy(gameObject);
    }
}