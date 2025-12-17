using UnityEngine;

public class PigeonWobble : MonoBehaviour
{
    public float bobHeight = 0.05f;
    public float bobSpeed = 6f;
    public float tiltAngle = 5f;
    public float tiltSpeed = 4f;

    Vector3 startLocalPos;
    Quaternion startLocalRot;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        float tilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;

        transform.localPosition = startLocalPos + new Vector3(0f, bob, 0f);
        transform.localRotation = startLocalRot * Quaternion.Euler(0f, 0f, tilt);
    }
}
