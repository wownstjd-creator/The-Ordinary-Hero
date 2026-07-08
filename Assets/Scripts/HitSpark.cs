using UnityEngine;

public class HitSpark : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.12f;
    [SerializeField] private float growSpeed = 8f;

    private float timer;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.localScale += Vector3.one * growSpeed * Time.deltaTime;

        if (sr != null)
        {
            Color color = sr.color;
            color.a = Mathf.Lerp(1f, 0f, timer / lifeTime);
            sr.color = color;
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}