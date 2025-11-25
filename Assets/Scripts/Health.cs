using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 50f;
    [HideInInspector] public float currentHealth;

    public System.Action onDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}