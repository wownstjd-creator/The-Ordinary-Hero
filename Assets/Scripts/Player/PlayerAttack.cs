using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDuration = 0.8f;
    [SerializeField] private float bladeSpawnTiming = 0.35f;

    [Header("Blade")]
    [SerializeField] private GameObject bladePrefab;
    [SerializeField] private Transform bladeSpawnPoint;

    [Header("Direction")]
    [SerializeField] private bool defaultFaceLeft = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool isAttacking;
    private bool hasSpawnedBlade;
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

        if (!hasSpawnedBlade && attackTimer >= bladeSpawnTiming)
        {
            SpawnBlade();
            hasSpawnedBlade = true;
        }

        if (attackTimer >= attackDuration)
        {
            EndAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasSpawnedBlade = false;
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

    private void SpawnBlade()
    {
        if (bladePrefab == null || bladeSpawnPoint == null)
        {
            Debug.LogWarning("Blade Prefab 또는 Blade Spawn Point가 비어있음");
            return;
        }

        int direction = GetFacingDirection();

        GameObject bladeObject = Instantiate(
            bladePrefab,
            bladeSpawnPoint.position,
            Quaternion.identity
        );

        Blade blade = bladeObject.GetComponent<Blade>();

        if (blade != null)
        {
            blade.Init(direction);
        }

        Debug.Log("Blade 생성");
    }

    private int GetFacingDirection()
    {
        // 네 프로젝트 현재 기준:
        // scale.x < 0 이 오른쪽
        return transform.localScale.x < 0 ? 1 : -1;
    }
}