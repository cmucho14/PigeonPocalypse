using UnityEngine;

public class MenuPigeonFly : MonoBehaviour
{
    [Header("Horizontal bounds")]
    public float leftX = -9f;
    public float rightX = 9f;

    [Header("Height range")]
    public float minHeight = 1.0f;
    public float maxHeight = 2.0f;

    [Header("Depth")]
    public float zDepth = -5f;

    [Header("Speed")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;

    // internal state
    float _direction;     // +1 = left → right, -1 = right → left
    float _speed;

    Transform _model;     // the visual child (Body)
    Vector3 _baseScale;

    void Awake()
    {
        // Find the MeshRenderer child (e.g., Body)
        var mr = GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            _model = mr.transform;
            _baseScale = _model.localScale;
        }
        else
        {
            // Fallback: use this transform
            _model = transform;
            _baseScale = transform.localScale;
        }
    }

    void OnEnable()
    {
        ResetBird();
    }

    void Update()
    {
        // Move horizontally
        transform.position += Vector3.right * _direction * _speed * Time.deltaTime;

        // If we’ve gone off screen, respawn on the opposite side
        if (_direction > 0f && transform.position.x > rightX + 1f ||
            _direction < 0f && transform.position.x < leftX - 1f)
        {
            ResetBird();
        }
    }

    void ResetBird()
    {
        // Pick random direction
        _direction = Random.value < 0.5f ? 1f : -1f;

        // Random speed
        _speed = Random.Range(minSpeed, maxSpeed);

        // Start X based on direction
        float startX = _direction > 0f ? leftX : rightX;

        // Random height
        float y = Random.Range(minHeight, maxHeight);

        // Place bird in world
        transform.position = new Vector3(startX, y, zDepth);

        // Make it face direction of travel
        ApplyFacing();
    }

    void ApplyFacing()
    {
        if (_model == null) return;

        float sign = _direction > 0f ? 1f : -1f;

        // Flip ONLY the X scale of the visual child, preserve original size
        _model.localScale = new Vector3(
            Mathf.Abs(_baseScale.x) * sign,
            _baseScale.y,
            _baseScale.z
        );
    }
}
