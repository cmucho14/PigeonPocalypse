using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public string endScreenScene = "EndScreenScene";
    
    private Health playerHealth;
    
    void Start()
    {
        // Fix lighting issues when loading from other scenes
        // Use coroutine to wait a frame so Unity can fully initialize
        StartCoroutine(FixLightingDelayed());
        
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
    
    IEnumerator FixLightingDelayed()
    {
        // Wait one frame for Unity to fully initialize the scene
        yield return null;
        
        // Force Unity to refresh lighting when scene loads
        // This fixes dark lighting issues when loading from start menu
        if (LightmapSettings.lightmaps != null && LightmapSettings.lightmaps.Length > 0)
        {
            // Force lightmap data to be reloaded
            LightmapSettings.lightmaps = LightmapSettings.lightmaps;
        }
        
        // Update light probes if they exist
        if (LightmapSettings.lightProbes != null)
        {
            // Force light probe update
            LightProbes.Tetrahedralize();
        }
        
        // Force a render update to apply lighting
        DynamicGI.UpdateEnvironment();
        
        // Wait another frame and update again to ensure lighting is applied
        yield return null;
        DynamicGI.UpdateEnvironment();
        
        Debug.Log("[GameManager] Lighting refreshed for scene: " + SceneManager.GetActiveScene().name);
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

