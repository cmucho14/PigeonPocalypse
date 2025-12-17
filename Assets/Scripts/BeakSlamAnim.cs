using System.Collections;
using UnityEngine;

public class BeakSlamAnim : MonoBehaviour
{
    [Header("Rotate THIS object downward (Pigeon or Body)")]
    public Transform visual;

    [Header("Slam Settings")]
    public float slamAngle = 45f;
    public float slamDownTime = 0.18f;
    public float slamHoldTime = 0.12f;
    public float slamUpTime = 0.30f;

    Quaternion startRot;
    Coroutine slamRoutine;
    bool isSlamming;

    // If you have PigeonWobble on the same object, we’ll disable it during the slam
    PigeonWobble wobble;

    void Awake()
    {
        if (visual == null) visual = transform;
        startRot = visual.localRotation;

        wobble = visual.GetComponent<PigeonWobble>();
        if (wobble == null) wobble = GetComponent<PigeonWobble>();
    }

    public void PlaySlam()
    {
        // Prevent double-trigger
        if (isSlamming) return;

        if (slamRoutine != null)
            StopCoroutine(slamRoutine);

        slamRoutine = StartCoroutine(Slam());
    }

    IEnumerator Slam()
    {
        isSlamming = true;

        if (wobble != null) wobble.enabled = false;

        Quaternion downRot = startRot * Quaternion.Euler(slamAngle, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, slamDownTime);
            visual.localRotation = Quaternion.Slerp(startRot, downRot, t);
            yield return null;
        }

        if (slamHoldTime > 0f)
            yield return new WaitForSeconds(slamHoldTime);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, slamUpTime);
            visual.localRotation = Quaternion.Slerp(downRot, startRot, t);
            yield return null;
        }

        visual.localRotation = startRot;

        if (wobble != null) wobble.enabled = true;

        slamRoutine = null;
        isSlamming = false;
    }
}
