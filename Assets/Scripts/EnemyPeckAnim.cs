using System.Collections;
using UnityEngine;

public class EnemyPeckAnim : MonoBehaviour
{
    [Header("Visual to animate (Pigeon or Body)")]
    public Transform visual;

    [Header("Hop")]
    public float hopHeight = 0.06f;
    public float hopTime = 0.08f;

    [Header("Peck")]
    public float peckAngle = 18f;
    public float peckTime = 0.05f;

    Vector3 startPos;
    Quaternion startRot;
    bool attacking;

    void Awake()
    {
        if (visual == null) visual = transform;
        startPos = visual.localPosition;
        startRot = visual.localRotation;
    }

    public void Play()
    {
        if (attacking) return;
        StartCoroutine(HopPeck());
    }

    IEnumerator HopPeck()
    {
        attacking = true;

        // Hop upward
        float t = 0f;
        Vector3 hopUp = startPos + Vector3.up * hopHeight;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, hopTime);
            visual.localPosition = Vector3.Lerp(startPos, hopUp, t);
            yield return null;
        }

        // Quick forward peck
        Quaternion peckRot = startRot * Quaternion.Euler(peckAngle, 0f, 0f);
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, peckTime);
            visual.localRotation = Quaternion.Slerp(startRot, peckRot, t);
            yield return null;
        }

        // Snap back
        visual.localPosition = startPos;
        visual.localRotation = startRot;

        attacking = false;
    }
}
