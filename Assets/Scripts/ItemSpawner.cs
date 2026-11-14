using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    [SerializeField] private GameObject oneHitShieldPrefab;
    [SerializeField] private GameObject timedShieldPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 12f;
    [SerializeField] private float spawnAreaRadius = 5f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem() 
    {
        GameObject itemToSpawn = (Random.value < 0.5f) ? oneHitShieldPrefab : timedShieldPrefab;

        if (itemToSpawn != null)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnAreaRadius;
            Instantiate(itemToSpawn, new Vector3(randomPos.x, randomPos.y, 0f), Quaternion.identity);
        }
    }
}
