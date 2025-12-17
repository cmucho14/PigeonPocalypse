using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDUI : MonoBehaviour
{
    public Slider healthBar;
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    private Health playerHealth;
    private PlayerXP playerXP;


    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogError("HUDUI: No Player found. Make sure Player object Tag = Player.");
            return;
        }

        playerHealth = player.GetComponent<Health>();
        playerXP = player.GetComponent<PlayerXP>();

        if (playerHealth != null)
        {
            playerHealth.onHealthChanged += OnHealthChanged;
            OnHealthChanged(playerHealth.currentHealth, playerHealth.maxHealth);
        }
        else Debug.LogError("HUDUI: Player missing Health.");

        if (playerXP != null)
        {
            playerXP.onXPChanged += OnXPChanged;
            OnXPChanged(playerXP.xp, playerXP.xpToNext, playerXP.level);
        }
        else Debug.LogError("HUDUI: Player missing PlayerXP.");
    }

    void OnDestroy()
    {
        if (playerHealth != null) playerHealth.onHealthChanged -= OnHealthChanged;
        if (playerXP != null) playerXP.onXPChanged -= OnXPChanged;
    }

    void OnHealthChanged(float current, float max)
    {
        if (healthBar) healthBar.value = (max <= 0f) ? 0f : current / max;
    }

    void OnXPChanged(int xp, int xpToNext, int level)
    {
        if (xpBar) xpBar.value = (xpToNext <= 0) ? 0f : (float)xp / xpToNext;
        if (levelText) levelText.text = $"LEVEL {level}";
    }
}
    
