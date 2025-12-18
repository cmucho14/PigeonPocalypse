using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures lighting is properly initialized when the game scene loads.
/// Fixes dark lighting issues when loading from other scenes.
/// </summary>
public class LightingFix : MonoBehaviour
{
    void Start()
    {
        // Force Unity to refresh lighting when scene loads
        // This fixes the issue where lighting is dark when loading from start menu
        RefreshLighting();
    }
    
    void RefreshLighting()
    {
        // Force lightmap data to be loaded
        LightmapSettings.lightmaps = LightmapSettings.lightmaps;
        
        // Update light probes if they exist
        if (LightmapSettings.lightProbes != null)
        {
            // Force light probe update
            LightProbes.Tetrahedralize();
        }
        
        // Force a render update to apply lighting
        DynamicGI.UpdateEnvironment();
        
        Debug.Log("[LightingFix] Lighting refreshed for scene: " + SceneManager.GetActiveScene().name);
    }
    
    // Also refresh when scene becomes active (in case of additive loading)
    void OnEnable()
    {
        RefreshLighting();
    }
}

