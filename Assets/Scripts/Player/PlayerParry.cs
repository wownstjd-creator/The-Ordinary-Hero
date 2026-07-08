using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry")]
    [SerializeField] private float parryDuration = 0.25f;
    [SerializeField] private float parryRange = 1.6f;
    [SerializeField] private float parryCooldown = 0.8f;
    [SerializeField] private float failedParryLockTime = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isParrying;
    private bool isLockedByFailedParry;
    private float parryTimer;
    private float cooldownTimer;
    private float failedLockTimer;

    private PlayerFocus playerFocus;

    public bool IsParrying => isParrying;
    public bool IsLockedByFailedParry => isLockedByFailedParry;

    private void Awake()
    {
        playerFocus = GetComponent<PlayerFocus>();
    }

    public void OnParry(InputValue value)
    {
        if (!value.isPressed) return;
        if (playerFocus != null && playerFocus.IsFocusing) return;
        if (isParrying) return;
        if (isLockedByFailedParry) return;
        if (cooldownTimer > 0f) return;

        StartParry();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isParrying)
        {
            parryTimer -= Time.deltaTime;

            if (parryTimer <= 0f)
            {
                EndParry(false);
            }
        }

        if (isLockedByFailedParry)
        {
            failedLockTimer -= Time.deltaTime;

            if (failedLockTimer <= 0f)
            {
                isLockedByFailedParry = false;
            }
        }
    }

    private void StartParry()
    {
        isParrying = true;
        parryTimer = parryDuration;
        cooldownTimer = parryCooldown;

        bool success = TryParryEnemy();

        if (success)
        {
            EndParry(true);
        }

        Debug.Log("패링 시도");
    }

    private void EndParry(bool success)
    {
        isParrying = false;

        if (!success)
        {
            isLockedByFailedParry = true;
            failedLockTimer = failedParryLockTime;
            Debug.Log("패링 실패 - 경직");
        }
    }

    private bool TryParryEnemy()
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

            if (goblin.IsParryable)
            {
                goblin.Stagger();
                Debug.Log("패링 성공!");
                if (HitStop.Instance != null)
                {
                    HitStop.Instance.Stop(0.07f);
                }
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, parryRange);
    }
}