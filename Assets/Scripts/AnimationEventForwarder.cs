using UnityEngine;

/// <summary>
/// Helper component that forwards animation events to PlayerAttack component
/// when the Animator is on a different GameObject than PlayerAttack
/// </summary>
public class AnimationEventForwarder : MonoBehaviour
{
    public PlayerAttack playerAttack;
    
    // This method will be called by animation events
    public void OnAttackHit()
    {
        if (playerAttack != null)
        {
            playerAttack.OnAttackHit();
        }
        else
        {
            Debug.LogWarning("AnimationEventForwarder: PlayerAttack reference is null!");
        }
    }
}

