using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    [Header("One-shot SFX Source")]
    public AudioSource sfxSource;

    [Header("Boss Intro")]
    public AudioClip bossIntroClip;


    [Header("Enemy Death")]
    public AudioClip enemyDeathClip;

    [Header("Level Up")]
    public AudioClip levelUpClip;

    [Header("Slash")]
    public AudioClip slashClip;

    [Header("Random Pigeon Ambient")]
    public AudioClip[] pigeonClips;
    public float pigeonMinDelay = 6f;
    public float pigeonMaxDelay = 14f;
    [Range(0f, 1f)] public float pigeonChance = 0.8f;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        // If you want it to persist across scenes, uncomment:
        // DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    void Start()
    {
        StartCoroutine(PigeonLoop());
    }

    public void PlayEnemyDeath()
    {
        PlayOneShot(enemyDeathClip);
    }

    public void PlayLevelUp()
    {
        PlayOneShot(levelUpClip);
    }

    public void PlaySlash()
    {
        PlayOneShot(slashClip);
    }

    public void PlayRandomPigeon()
    {
        if (pigeonClips == null || pigeonClips.Length == 0) return;
        var clip = pigeonClips[Random.Range(0, pigeonClips.Length)];
        PlayOneShot(clip);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    IEnumerator PigeonLoop()
    {
        while (true)
        {
            float wait = Random.Range(pigeonMinDelay, pigeonMaxDelay);
            yield return new WaitForSeconds(wait);

            if (Random.value <= pigeonChance)
                PlayRandomPigeon();
        }
    }
    public void PlayBossIntro()
    {
        if (sfxSource != null && bossIntroClip != null)
        {
            sfxSource.PlayOneShot(bossIntroClip);
        }
    }
}



