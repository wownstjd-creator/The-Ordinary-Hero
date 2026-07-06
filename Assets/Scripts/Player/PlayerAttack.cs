using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackDuration = 0.45f;
    [SerializeField] private float hitTiming = 0.18f;

    [Header("Hit Box")]
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1.2f, 1f);
    [SerializeField] private Vector2 hitBoxOffset = new Vector2(0.8f, 0f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isAttacking;
    private bool hasHit;
    private float attackTimer;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (isAttacking) return;

        StartAttack();
    }

    private void Update()
    {
        if (!isAttacking) return;

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
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasHit = false;
        attackTimer = 0f;

        if (animator != null)
        {
            animator.SetTrigger("2_Attack");
        }

        Debug.Log("공격 시작");
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("공격 종료");
    }

    private void CheckHit()
    {
        Vector2 center = GetHitBoxCenter();

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            hitBoxSize,
            0f,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();

            if (enemy == null)
            {
                enemy = hit.GetComponentInParent<EnemyHealth>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Debug.Log($"공격 판정: {hits.Length}개 감지");
    }

    private Vector2 GetHitBoxCenter()
    {
        float direction = transform.localScale.x < 0 ? 1f : -1f;

        return (Vector2)transform.position + new Vector2(
            hitBoxOffset.x * direction,
            hitBoxOffset.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetHitBoxCenter(), hitBoxSize);
    }
}