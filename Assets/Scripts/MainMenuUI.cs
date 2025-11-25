using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "Game";       // your main play scene
    public string upgradesSceneName = "Upgrades"; // optional, if you have one

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnUpgradesClicked()
    {
        if (!string.IsNullOrEmpty(upgradesSceneName))
            SceneManager.LoadScene(upgradesSceneName);
        else
            Debug.Log("Upgrades scene not set yet.");
    }

    public void OnSettingsClicked()
    {
        // TODO: open settings panel later
        Debug.Log("Settings clicked");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
