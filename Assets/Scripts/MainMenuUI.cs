using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "TestMap";
    public string upgradesSceneName = ""; // you don't have this yet

    public void OnPlayClicked()
    {
        Time.timeScale = 1f;
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
