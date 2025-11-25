using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    public string gameSceneName = "Game";
    public string mainMenuSceneName = "MainMenu";

    [Header("Stats")]
    public TextMeshProUGUI wavesText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI xpText;

    public void SetStats(int waves, int enemies, int xp)
    {
        if (wavesText)    wavesText.text    = $"Waves Survived: {waves}";
        if (enemiesText)  enemiesText.text  = $"Enemies Defeated: {enemies}";
        if (xpText)       xpText.text       = $"XP Earned: {xp}";
    }

    public void OnRetryClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnUpgradesClicked()
    {
        // later, go to upgrades scene
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}


