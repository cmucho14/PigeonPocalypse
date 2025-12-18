using UnityEngine;

public class BossIntroTrigger : MonoBehaviour
{
    private bool played = false;

    private void OnTriggerEnter(Collider other)
    {
        if (played) return;

        if (other.CompareTag("Player"))
        {
            played = true;

            if (AudioManager.I != null)
                AudioManager.I.PlayBossIntro();
        }
    }
}

