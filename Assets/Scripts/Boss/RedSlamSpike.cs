using UnityEngine;

public class RedSlamSpike : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 2;
    [SerializeField] private LayerMask playerLayer;

    [Header("Life")]
    [SerializeField] private float lifeTime = 0.35f;

    private bool hasHit;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (((1 << other.gameObject.layer) & playerLayer) == 0)
        {
            return;
        }

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerHealth>();
        }

        if (player != null)
        {
            player.TakeDamage(damage);
            hasHit = true;
        }
    }
}