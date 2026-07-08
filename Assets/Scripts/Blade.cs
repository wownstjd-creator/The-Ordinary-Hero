using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Blade : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 0.25f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitSparkPrefab;

    [Header("Sound")]
    [SerializeField] private AudioClip hitSound;

    private Rigidbody2D rb;
    private int direction = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(int dir)
    {
        direction = dir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction * speed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0)
        {
            return;
        }

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            enemy = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            SpawnHitSpark(other.transform.position);
            PlayHitSound();
        }

        Destroy(gameObject);
    }

    private void SpawnHitSpark(Vector3 position)
    {
        if (hitSparkPrefab == null) return;

        Instantiate(
            hitSparkPrefab,
            position + new Vector3(0f, 0.4f, 0f),
            Quaternion.identity
        );
    }

    private void PlayHitSound()
    {
        if (hitSound == null) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hitSound);
        }
    }
}