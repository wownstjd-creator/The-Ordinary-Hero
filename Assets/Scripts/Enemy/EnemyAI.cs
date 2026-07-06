using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool defaultFaceLeft = true;

    private Rigidbody2D rb;
    private EnemyAttack enemyAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (enemyAttack != null)
        {
            enemyAttack.SetTarget(target);
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        if (enemyAttack != null && enemyAttack.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            SetMoveAnimation(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            SetMoveAnimation(false);
            return;
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if (direction.x > 0.01f)
        {
            FaceRight();
        }
        else if (direction.x < -0.01f)
        {
            FaceLeft();
        }

        SetMoveAnimation(true);
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
}