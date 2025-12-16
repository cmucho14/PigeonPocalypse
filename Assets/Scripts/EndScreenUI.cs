using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    public string gameSceneName = "TestMap";
    public string mainMenuSceneName = "StartMenuScene";

    [Header("Stats")]
    public TextMeshProUGUI wavesText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI xpText;

    public void SetStats(int waves, int enemies, int xp)
    {
        if (wavesText)   wavesText.text   = $"Waves Survived: {waves}";
        if (enemiesText) enemiesText.text = $"Enemies Defeated: {enemies}";
        if (xpText)      xpText.text      = $"XP Earned: {xp}";
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;                 // important if you paused on death
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
