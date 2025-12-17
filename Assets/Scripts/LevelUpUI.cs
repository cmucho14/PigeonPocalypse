using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public GameObject levelUpPanel;

    [Header("Upgrade amounts")]
    public float speedIncrease = 1f;
    public float healthIncrease = 10f;
    public float damageIncrease = 0.25f;

    PlayerXP playerXP;
    PlayerMovement playerMove;
    Health playerHealth;
    PlayerCombatStats playerCombat;

    bool choosing = false;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogError("LevelUpUI: No Player tagged 'Player'.");
            return;
        }

        playerXP = player.GetComponent<PlayerXP>();
        playerMove = player.GetComponent<PlayerMovement>();
        playerHealth = player.GetComponent<Health>();
        playerCombat = player.GetComponent<PlayerCombatStats>();

        if (playerXP != null) playerXP.onLevelUp += OnLevelUp;

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (playerXP != null) playerXP.onLevelUp -= OnLevelUp;
    }

    void OnLevelUp(int newLevel)
    {
        if (choosing) return;

        choosing = true;
        Time.timeScale = 0f;

        if (levelUpPanel != null) levelUpPanel.SetActive(true);

        Debug.Log("LEVEL UP -> " + newLevel);
    }

    public void ChooseSpeed()
    {
        if (playerMove != null)
            playerMove.moveSpeed += speedIncrease;

        Close();
    }

    public void ChooseHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.maxHealth += healthIncrease;
            playerHealth.currentHealth = playerHealth.maxHealth;
            playerHealth.onHealthChanged?.Invoke(playerHealth.currentHealth, playerHealth.maxHealth);
        }

        Close();
    }

    public void ChooseDamage()
    {
        if (playerCombat != null)
            playerCombat.UpgradeDamage(damageIncrease);

        Close();
    }

    void Close()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
        choosing = false;
    }
}

