using System.Collections;
using UnityEngine;

public class Bear : MonoBehaviour
{
    [Header("Bear Stats")]
    [SerializeField] private float speed = 1.6f; 
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private int scoreReward = 5;
    [SerializeField] private int yurtDamage = 2;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 1.2f;

    [Header("UI & VFX Feedback")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject deathVfxPrefab;
    [SerializeField] private Color scorePopupColor = new Color(1f, 0.9f, 0.35f);
    [SerializeField] private Color damagePopupColor = new Color(1f, 0.35f, 0.35f);

    private int currentHealth;
    private Transform yurtTarget;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool insInvulnerable = false;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        GameObject yurtObj = GameObject.FindWithTag("Yurt");
        if (yurtObj != null)
        {
            yurtTarget = yurtObj.transform;
        }
    }

    private void Update()
    {
        if (yurtTarget == null) return;

        Vector3 direction = (yurtTarget.position - transform.position).normalized;
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (insInvulnerable) return;

            TakeHit(other.transform.position);
        }
        else if (other.CompareTag("Yurt"))
        {
            bool blockedByShield = GameManager.Instance != null && GameManager.Instance.TakeDamage(yurtDamage);
            if (!blockedByShield)
            {
                AudioManager.Instance?.PlayYurtDamage();
                SpawnText("-" + yurtDamage, damagePopupColor);
            }
            else
            {
                AudioManager.Instance?.PlayEnemyHit();
            }

            if (deathVfxPrefab != null) Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void TakeHit(Vector3 attackerPosition)
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            AudioManager.Instance?.PlayEnemyDeath();
            SpawnText("+" + scoreReward, scorePopupColor);
            GameManager.Instance?.AddScore(scoreReward);
            if (deathVfxPrefab != null) Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            AudioManager.Instance?.PlayEnemyHit();
            StartCoroutine(HitFeedbackRoutine(attackerPosition));
        }
    }

    private IEnumerator HitFeedbackRoutine(Vector3 attackerPosition)
    {
        insInvulnerable = true;

        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        transform.position += pushDirection * knockbackForce;

        if (spriteRenderer != null) 
        {
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(0.12f);

        if (spriteRenderer != null) 
        {
            spriteRenderer.color = originalColor;
        }

        insInvulnerable = false;
    }

    private void SpawnText(string text, Color color)
    {
        if (floatingTextPrefab == null) return;
        GameObject obj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        FloatingText ft = obj.GetComponent<FloatingText>();
        if (ft != null) ft.Setup(text, color);
    }
}