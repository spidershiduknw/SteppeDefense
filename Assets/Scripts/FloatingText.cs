using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float destroyTime = 0.8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.7f, 0f);
    [SerializeField] private Vector3 randomizeIntensity = new Vector3(0.35f, 0.2f, 0f);
    [SerializeField] private float floatSpeed = 1.2f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
        transform.localPosition += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            0f
        );
    }

    private void Update()
    {
        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
    }

    public void Setup(string text, Color color)
    {
        TMP_Text tmp = GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }
    }

    public void Setup(int amount, Color color)
    {
        Setup($"+{amount}", color);
    }
}