using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GoblinStateMachine : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.3f;
    [SerializeField] private float attackRange = 1.4f;

    [Header("Attack Timing")]
    [SerializeField] private float attackReadyTime = 0.8f;
    [SerializeField] private float attackHitTime = 0.25f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float recoveryTime = 0.8f;

    [Header("Attack HitBox")]
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1.2f, 1f);
    [SerializeField] private Vector2 hitBoxOffset = new Vector2(0.8f, 0f);
    [SerializeField] private LayerMask playerLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool defaultFaceLeft = true;

    [SerializeField] private float staggerTime = 1.2f;
    private Rigidbody2D rb;
    private EnemyState currentState;
    private float stateTimer;
    private bool hasHit;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        ChangeState(EnemyState.Chase);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.AttackReady:
                UpdateAttackReady();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Recovery:
                UpdateRecovery();
                break;
            case EnemyState.Stagger:
                UpdateStagger();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentState != EnemyState.Chase)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateChase()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.AttackReady);
            return;
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (direction.x > 0.01f) FaceRight();
        else if (direction.x < -0.01f) FaceLeft();

        SetMoveAnimation(true);
    }

    private void UpdateAttackReady()
    {
        SetMoveAnimation(false);

        if (stateTimer >= attackReadyTime)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void UpdateAttack()
    {
        if (!hasHit && stateTimer >= attackHitTime)
        {
            CheckHit();
            hasHit = true;
        }

        if (stateTimer >= attackDuration)
        {
            ChangeState(EnemyState.Recovery);
        }
    }

    private void UpdateRecovery()
    {
        if (stateTimer >= recoveryTime)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        hasHit = false;

        Debug.Log($"Goblin State: {newState}");

        if (animator == null) return;

        animator.SetBool("1_Move", false);

        if (newState == EnemyState.Chase)
        {
            animator.SetBool("1_Move", true);
        }
        else if (newState == EnemyState.AttackReady)
        {
            // 지금은 대기 모션 사용. 나중에 전용 준비 모션 있으면 바꾼다.
        }
        else if (newState == EnemyState.Attack)
        {
            animator.SetTrigger("2_Attack");
        }
        else if (newState == EnemyState.Stagger)
        {
            animator.SetBool("1_Move", false);
        }
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

    public void Die()
    {
        ChangeState(EnemyState.Dead);
        rb.linearVelocity = Vector2.zero;
    }

    private void SetMoveAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("1_Move", isMoving);
        }
    }

    private void FaceRight()
    {
        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetHitBoxCenter(), hitBoxSize);
    }
    private void UpdateStagger()
    {
        if (stateTimer >= staggerTime)
        {
            ChangeState(EnemyState.Chase);
        }
    }
    public void Stagger()
    {
        ChangeState(EnemyState.Stagger);
    }
}