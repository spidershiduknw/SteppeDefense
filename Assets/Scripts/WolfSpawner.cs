using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private GameObject bearPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float baseInterval = 3.0f;
    [SerializeField] private float spawnRadius = 11.0f;
    [SerializeField] [Range(0f, 1f)] private float bearChance = 0.25f;

    private float timer = 0f;
    private float totalSurvivalTime = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        totalSurvivalTime += Time.deltaTime;

        float currentInterval = Mathf.Max(0.6f, baseInterval - (totalSurvivalTime * 0.04f));

        if (timer >= currentInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 spawnPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;

        GameObject prefabToSpawn = (Random.value < bearChance && bearPrefab != null) ? bearPrefab : wolfPrefab;

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity); 
        }
    }
}