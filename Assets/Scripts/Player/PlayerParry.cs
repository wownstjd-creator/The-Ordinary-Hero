using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry")]
    [SerializeField] private float parryDuration = 0.25f;
    [SerializeField] private float parryRange = 1.6f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isParrying;
    private float parryTimer;

    public bool IsParrying => isParrying;
    private PlayerFocus playerFocus;

    private void Awake()
    {
        playerFocus = GetComponent<PlayerFocus>();
    }

    public void OnParry(InputValue value)
    {
        if (!value.isPressed) return;
        if (playerFocus != null && playerFocus.IsFocusing) return;
        if (isParrying) return;

        StartParry();
    }

    private void Update()
    {
        if (!isParrying) return;

        parryTimer -= Time.deltaTime;

        if (parryTimer <= 0f)
        {
            isParrying = false;
        }
    }

    private void StartParry()
    {
        isParrying = true;
        parryTimer = parryDuration;

        TryParryEnemy();

        Debug.Log("패링 시도");
    }

    private void TryParryEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            parryRange,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            GoblinStateMachine goblin = hit.GetComponent<GoblinStateMachine>();

            if (goblin == null)
            {
                goblin = hit.GetComponentInParent<GoblinStateMachine>();
            }

            if (goblin == null) continue;

            if (goblin.CurrentState == EnemyState.Attack)
            {
                goblin.Stagger();
                Debug.Log("패링 성공!");
                return;
            }
        }

        Debug.Log("패링 실패");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, parryRange);
    }
}