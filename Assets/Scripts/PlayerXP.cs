using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int xp = 0;
    public int xpToNext = 25;

    public System.Action<int, int, int> onXPChanged;
    public System.Action<int> onLevelUp;

    private void Start()
    {
        onXPChanged?.Invoke(xp, xpToNext, level);
    }

    public void AddXP(int amount)
    {
        xp += amount;

        bool leveledUp = false;

        while (xp >= xpToNext)
        {
            xp -= xpToNext;
            level++;
            xpToNext = Mathf.RoundToInt(xpToNext * 1.25f);
            leveledUp = true;
        }

        onXPChanged?.Invoke(xp, xpToNext, level);

        if (leveledUp)
        {
            // ✅ Level up sound
            if (AudioManager.I != null)
                AudioManager.I.PlayLevelUp();

            Debug.Log($"[LEVEL UP EVENT] Level is now {level}");
            onLevelUp?.Invoke(level);
            Debug.Log("[LEVEL UP EVENT] Handler finished");
        }
    }
}

