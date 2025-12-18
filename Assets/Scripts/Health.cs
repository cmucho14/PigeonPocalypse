using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 50f;
    [HideInInspector] public float currentHealth;

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

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Award XP if this object has an EnemyXPReward component
        EnemyXPReward reward = GetComponent<EnemyXPReward>();
        if (reward != null && reward.xpOnDeath > 0)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerXP xp = playerObj.GetComponent<PlayerXP>();
                if (xp != null)
                {
                    xp.AddXP(reward.xpOnDeath);
                }
            }
        }

        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
