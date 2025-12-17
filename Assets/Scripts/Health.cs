using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 50f;
    [HideInInspector] public float currentHealth;

    public System.Action onDeath;
    public System.Action<float, float> onHealthChanged; // current, max

    private void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}

