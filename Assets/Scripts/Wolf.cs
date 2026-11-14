using UnityEngine;

public class Wolf : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private int scoreReward = 5;
    [SerializeField] private int yurtDamage = 1;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject deathVfxPrefab;
    [SerializeField] private Color scorePopupColor = new Color(1f, 0.9f, 0.35f);
    [SerializeField] private Color damagePopupColor = new Color(1f, 0.35f, 0.35f);

    private Transform yurtTarget;

    private void Start()
    {
        GameObject yurtObj = GameObject.FindWithTag("Yurt");
        if (yurtObj != null)
        {
            yurtTarget = yurtObj.transform;
        }
    }

    private void Update()
    {
        if (yurtTarget == null) return;

        Vector3 moveDirection = (yurtTarget.position - transform.position).normalized;
        transform.position += moveDirection * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayEnemyDeath();
            SpawnText("+" + scoreReward, scorePopupColor);
            GameManager.Instance?.AddScore(scoreReward);
            if (deathVfxPrefab != null) Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
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

    private void SpawnText(string text, Color color)
    {
        if (floatingTextPrefab == null) return;
        GameObject obj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        FloatingText ft = obj.GetComponent<FloatingText>();
        if (ft != null) ft.Setup(text, color);
    }
}