using UnityEngine;

public enum ItemType { OneHitShield, TimedShield }

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private float lifetime = 8f;

    [Header("Rotation & Thickness")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float minThickness = 0.2f;
    private float currentAngle = 0f;

    [Header("Bouncing Motion")]
    [SerializeField] private float bounceAmplitude = 0.18f;
    [SerializeField] private float bounceFrequency = 4f;

    private Vector3 basePosition;
    private Vector3 initialScale;

    private void Start()
    {
        basePosition = transform.position;
        initialScale = transform.localScale;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;

        float widthFactor = Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float absoluteWidth = Mathf.Max(Mathf.Abs(widthFactor), minThickness);

        transform.localScale = new Vector3(initialScale.x * absoluteWidth, initialScale.y, initialScale.z);

        float newY = basePosition.y + Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;
        transform.position = new Vector3(basePosition.x, newY, basePosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Yurt"))
        {
            if (itemType == ItemType.OneHitShield)
            {
                GameManager.Instance?.ActivateOneHitShield();
                AudioManager.Instance?.PlayPickupAaruul();
            }
            else if (itemType == ItemType.TimedShield)
            {
                GameManager.Instance?.ActivateTimedShield(10.0f);
                AudioManager.Instance?.PlayPickupBell();
            }

            Destroy(gameObject);
        }
    }
}