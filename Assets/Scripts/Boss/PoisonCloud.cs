using System.Collections;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float tickInterval = 0.8f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Life")]
    [SerializeField] private float lifeTime = 5f;

    private bool playerInside;
    private PlayerHealth targetPlayer;
    private Coroutine damageRoutine;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerHealth>();
        }

        if (player == null) return;

        playerInside = true;
        targetPlayer = player;

        if (damageRoutine == null)
        {
            damageRoutine = StartCoroutine(DamageRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerHealth>();
        }

        if (player != targetPlayer) return;

        playerInside = false;
        targetPlayer = null;

        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DamageRoutine()
    {
        while (playerInside && targetPlayer != null)
        {
            targetPlayer.TakeDamage(damage);
            Debug.Log("독안개 틱 데미지");

            yield return new WaitForSeconds(tickInterval);
        }

        damageRoutine = null;
    }
}