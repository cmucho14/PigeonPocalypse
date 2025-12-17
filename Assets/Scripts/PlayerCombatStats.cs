using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    public float damageMultiplier = 1f;

    public void UpgradeDamage(float amount)
    {
        damageMultiplier += amount;
        Debug.Log("Damage multiplier -> " + damageMultiplier);
    }
}
