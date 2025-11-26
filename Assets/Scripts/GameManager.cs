using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string endScreenScene = "EndScreenScene";
    
    private Health playerHealth;
    
    void Start()
    {
        // Find the player and subscribe to death event
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            
            if (playerHealth != null)
            {
                playerHealth.onDeath += OnPlayerDeath;
            }
            else
            {
                Debug.LogWarning("GameManager: Player has no Health component!");
            }
        }
        else
        {
            Debug.LogWarning("GameManager: No object with 'Player' tag found!");
        }
    }
    
    void OnPlayerDeath()
    {
        Debug.Log("Player died! Loading end screen...");
        SceneManager.LoadScene(endScreenScene);
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent errors
        if (playerHealth != null)
        {
            playerHealth.onDeath -= OnPlayerDeath;
        }
    }
}

