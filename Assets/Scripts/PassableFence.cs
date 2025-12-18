using UnityEngine;

/// <summary>
/// Attach this script to fence objects to make them passable by the player.
/// The fence will still be visible but won't block movement.
/// </summary>
public class PassableFence : MonoBehaviour
{
    void Start()
    {
        // Find all colliders on this object and its children
        Collider[] colliders = GetComponentsInChildren<Collider>();
        
        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning($"PassableFence on {gameObject.name}: No player found with 'Player' tag. Fence will still block.");
            return;
        }
        
        Collider[] playerColliders = playerObj.GetComponentsInChildren<Collider>();
        
        // Ignore collisions between fence and player
        foreach (Collider fenceCol in colliders)
        {
            foreach (Collider playerCol in playerColliders)
            {
                if (fenceCol != null && playerCol != null)
                {
                    Physics.IgnoreCollision(fenceCol, playerCol, true);
                }
            }
        }
        
        Debug.Log($"PassableFence: Made {gameObject.name} passable for player.");
    }
}

