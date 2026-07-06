using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange = 1.3f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float hitTiming = 0.45f;
    [SerializeField] private float attackDuration = 0.8f;

    [Header("Hit Box")]
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1.2f, 1f);
    [SerializeField] private Vector2 hitBoxOffset = new Vector2(0.8f, 0f);
    [SerializeField] private LayerMask playerLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Transform target;
    private float cooldownTimer;
    private float attackTimer;
    private bool isAttacking;
    private bool hasHit;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (!hasHit && attackTimer >= hitTiming)
            {
                CheckHit();
                hasHit = true;
            }

            if (attackTimer >= attackDuration)
            {
                EndAttack();
            }

            return;
        }

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange && cooldownTimer <= 0f)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasHit = false;
        attackTimer = 0f;
        cooldownTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("2_Attack");
        }

        Debug.Log("고블린 공격 시작");
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("고블린 공격 종료");
    }

    private void CheckHit()
    {
        Vector2 center = GetHitBoxCenter();

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            hitBoxSize,
            0f,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();

            if (player == null)
            {
                player = hit.GetComponentInParent<PlayerHealth>();
            }

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        Debug.Log($"고블린 공격 판정: {hits.Length}개");
    }

    private Vector2 GetHitBoxCenter()
    {
        float direction = transform.localScale.x < 0 ? -1f : 1f;

        return (Vector2)transform.position + new Vector2(
            hitBoxOffset.x * direction,
            hitBoxOffset.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetHitBoxCenter(), hitBoxSize);
    }
}