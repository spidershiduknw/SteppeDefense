using UnityEngine;

public class SimpleSpriteAnimator : MonoBehaviour
{
    [Header("Sprite Configuration")]
    [SerializeField] private Sprite idleFrame;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private float frameRate = 8f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float timer;
    private int currentFrame;
    
    private Vector3 lastPosition; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position; 
    }

    private void Update()
    {
        bool isMoving = false;

        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.05f)
        {
            isMoving = true;
        }
        else if (Vector3.Distance(transform.position, lastPosition) > 0.001f)
        {
            isMoving = true;
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0) isMoving = true;
        }

        lastPosition = transform.position; 

        if (isMoving && walkFrames != null && walkFrames.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / frameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % walkFrames.Length;
                spriteRenderer.sprite = walkFrames[currentFrame];
            }
        }
        else if (idleFrame != null)
        {
            spriteRenderer.sprite = idleFrame;
            timer = 0f;
            currentFrame = 0;
        }
    }
}