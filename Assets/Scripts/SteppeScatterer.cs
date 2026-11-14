using UnityEngine;

public class SteppeScatterer : MonoBehaviour
{
    [Header("Sprites to Scatter")]
    public Sprite[] detailSprites;

    [Header("Spawn Settings")]
    public int count = 45;
    public Vector2 areaSize = new Vector2(16f, 9f);
    public float centerClearRadius = 2.2f;
    public int orderInLayer = 0;

    [Header("Scale Variation")]
    public float minScale = 0.65f;
    public float maxScale = 1.05f;

    void Start()
    {
        Scatter();
    }

    [ContextMenu("Scatter Now")]
    public void Scatter()
    {
        // Clear any previously spawned details
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        if (detailSprites == null || detailSprites.Length == 0) return;

        int spawned = 0;
        int safety = 0;

        while (spawned < count && safety < count * 10)
        {
            safety++;
            Vector2 pos = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            // Keep the area around the Yurt clear
            if (pos.magnitude < centerClearRadius) continue;

            GameObject obj = new GameObject("SteppeDetail_" + spawned);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = pos;

            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = detailSprites[Random.Range(0, detailSprites.Length)];
            sr.sortingOrder = orderInLayer;
            sr.flipX = Random.value > 0.5f;

            spawned++;
        }
    }
}