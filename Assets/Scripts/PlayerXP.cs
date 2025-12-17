using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;
    public int xp = 0;
    public int xpToNext = 25;

    public System.Action<int, int, int> onXPChanged; // xp, xpToNext, level

    private void Start()
    {
        onXPChanged?.Invoke(xp, xpToNext, level);
    }

    public void AddXP(int amount)
    {
        xp += amount;

        while (xp >= xpToNext)
        {
            xp -= xpToNext;
            level++;
            xpToNext = Mathf.RoundToInt(xpToNext * 1.25f);
        }

        onXPChanged?.Invoke(xp, xpToNext, level);
    }
}
