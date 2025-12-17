using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int xp = 0;
    public int xpToNext = 25;

    public System.Action<int, int, int> onXPChanged; // xp, xpToNext, level
    public System.Action<int> onLevelUp;             // NEW: fires when level increases (passes new level)

    private void Start()
    {
        onXPChanged?.Invoke(xp, xpToNext, level);
    }

    public void AddXP(int amount)
    {
        Debug.Log($"[XP GAIN] Player gained {amount} XP");

        xp += amount;
        bool leveledUp = false;

        while (xp >= xpToNext)
        {
            xp -= xpToNext;
            level++;
            xpToNext = Mathf.RoundToInt(xpToNext * 1.25f);
            leveledUp = true;

            Debug.Log($"[LEVEL UP] Player is now level {level}");
        }

        Debug.Log($"[XP STATUS] XP: {xp}/{xpToNext} | Level: {level}");

        onXPChanged?.Invoke(xp, xpToNext, level);

        if (leveledUp)
            onLevelUp?.Invoke(level);
    }
}
