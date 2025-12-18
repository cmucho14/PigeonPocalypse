using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
public class BossWinOnDeath : MonoBehaviour
{
    public string winSceneName = "WinScreenScene";

    private Health h;
    private bool fired;

    void Awake()
    {
        h = GetComponent<Health>();
        h.onDeath += HandleBossDeath;
    }

    void OnDestroy()
    {
        if (h != null) h.onDeath -= HandleBossDeath;
    }

    void HandleBossDeath()
    {
        if (fired) return;
        fired = true;

        Debug.Log("[WIN] Boss defeated -> loading win scene");

        // Just in case something paused time (level up UI etc.)
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(winSceneName);
    }
}
