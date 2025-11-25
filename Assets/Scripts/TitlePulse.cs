using UnityEngine;

public class TitlePulse : MonoBehaviour
{
    public float scaleAmount = 0.05f;
    public float speed = 1.5f;

    private Vector3 _baseScale;

    void Start()
    {
        _baseScale = transform.localScale;
    }

    void Update()
    {
        float s = 1f + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = _baseScale * s;
    }
}

