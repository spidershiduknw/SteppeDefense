using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float padding = 0.5f;

    private Vector2 inputVector;
    private SpriteRenderer sr;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        }
    }

    private void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        if (inputVector.x < 0) sr.flipX = true;
        else if (inputVector.x > 0) sr.flipX = false;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = inputVector.normalized * moveSpeed;
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x + padding, maxBounds.x - padding);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + padding, maxBounds.y - padding);
        transform.position = pos;
    }
}