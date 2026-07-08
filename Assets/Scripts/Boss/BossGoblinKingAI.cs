using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossGoblinKingAI : MonoBehaviour
{
    private enum BossState
    {
        Idle,
        Approach,
        SkillReady,
        BlueCharge,
        RedSlam,
        PurpleSummon,
        GreenPoison,
        Recovery
    }

    private enum BossSkill
    {
        None,
        BlueCharge,
        RedSlam,
        PurpleSummon,
        GreenPoison,
    }
    [Header("Green Skill - Poison")]
    [SerializeField] private GameObject poisonCloudPrefab;
    [SerializeField] private Transform poisonSpawnPoint;
    [SerializeField] private float poisonRecoveryTime = 1.2f;
    [SerializeField] private float poisonRange = 6f;
    private bool hasPoisoned;
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Wake")]
    [SerializeField] private bool startAwake = false;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.4f;
    [SerializeField] private float stopDistance = 2.5f;

    [Header("Skill Range")]
    [SerializeField] private float blueChargeRange = 7f;
    [SerializeField] private float redSlamRange = 2.8f;
    [SerializeField] private float purpleSummonRange = 8f;

    [Header("Timing")]
    [SerializeField] private float skillReadyTime = 1.8f;
    [SerializeField] private float recoveryTime = 1.0f;

    [Header("Blue Skill - Charge")]
    [SerializeField] private float chargeSpeed = 8f;
    [SerializeField] private float chargeTime = 0.45f;
    [SerializeField] private int chargeDamage = 1;
    [SerializeField] private Vector2 chargeHitBoxSize = new Vector2(2.5f, 1.3f);

    [Header("Red Skill - Slam")]
    [SerializeField] private int slamDamage = 2;
    [SerializeField] private Vector2 slamHitBoxSize = new Vector2(4f, 1.5f);
    [SerializeField] private Vector2 slamHitBoxOffset = new Vector2(0f, -0.2f);

    [Header("Red Skill Effect")]
    [SerializeField] private GameObject redSlamSpikePrefab;
    [SerializeField] private int redSpikeCount = 5;
    [SerializeField] private float redSpikeSpacing = 0.75f;
    [SerializeField] private float redSpikeInterval = 0.06f;
    [SerializeField] private Vector2 redSpikeStartOffset = new Vector2(1.0f, -0.8f);

    [Header("Purple Skill - Summon")]
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private Transform summonPoint;
    [SerializeField] private float summonRecoveryTime = 1.2f;

    [Header("Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Visual")]
    [SerializeField] private BossAuraIndicator auraIndicator;
    [SerializeField] private BossChargeWarningLine chargeWarningLine;

    private Rigidbody2D rb;
    private BossArenaController arenaController;

    private BossState currentState = BossState.Idle;
    private BossSkill currentSkill = BossSkill.None;
    private BossSkill lastSkill = BossSkill.None;

    private float stateTimer;
    private int facingDirection = -1;
    private bool isAwake;
    private bool hasHit;
    private bool hasSummoned;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (auraIndicator == null)
        {
            auraIndicator = GetComponentInChildren<BossAuraIndicator>(true);
        }

        if (chargeWarningLine == null)
        {
            chargeWarningLine = GetComponentInChildren<BossChargeWarningLine>(true);
        }

        isAwake = startAwake;
        ChangeState(isAwake ? BossState.Approach : BossState.Idle);
    }

    private void Update()
    {
        if (!isAwake) return;
        if (target == null) return;

        stateTimer += Time.deltaTime;
        UpdateFacing();

        switch (currentState)
        {
            case BossState.Approach:
                UpdateApproach();
                break;

            case BossState.SkillReady:
                UpdateSkillReady();
                break;

            case BossState.BlueCharge:
                UpdateBlueCharge();
                break;

            case BossState.RedSlam:
                UpdateRedSlam();
                break;

            case BossState.PurpleSummon:
                UpdatePurpleSummon();
                break;

            case BossState.Recovery:
                UpdateRecovery();
                break;
            case BossState.GreenPoison:
                UpdateGreenPoison();
                break;
        }
    }

    public void WakeUp()
    {
        if (isAwake) return;

        isAwake = true;
        ChangeState(BossState.Approach);

        Debug.Log("고블린 왕 전투 시작");
    }

    public void SetArena(BossArenaController arena)
    {
        arenaController = arena;
    }

    private void UpdateApproach()
    {
        float distance = GetDistanceToTarget();

        BossSkill possibleSkill = GetAvailableSkill(distance);

        if (possibleSkill != BossSkill.None)
        {
            PrepareSkill(possibleSkill);
            return;
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float moveX = direction.x * moveSpeed;

        if (arenaController != null && arenaController.IsArenaActive)
        {
            float safeMargin = 1.5f;
            float leftSafeX = arenaController.LeftX + safeMargin;
            float rightSafeX = arenaController.RightX - safeMargin;

            if (transform.position.x <= leftSafeX && moveX < 0)
            {
                moveX = 0;
            }

            if (transform.position.x >= rightSafeX && moveX > 0)
            {
                moveX = 0;
            }
        }

        rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
    }
    private void UpdateGreenPoison()
    {
        rb.linearVelocity = Vector2.zero;

        if (!hasPoisoned)
        {
            SpawnPoisonCloud();
            hasPoisoned = true;
        }

        if (stateTimer >= poisonRecoveryTime)
        {
            ChangeState(BossState.Recovery);
        }
    }

    private void SpawnPoisonCloud()
    {
        if (poisonCloudPrefab == null)
        {
            Debug.LogWarning("PoisonCloud Prefab이 비어있음");
            return;
        }

        Vector3 spawnPos;

        if (poisonSpawnPoint != null)
        {
            spawnPos = poisonSpawnPoint.position;
        }
        else
        {
            spawnPos = transform.position + new Vector3(facingDirection * 1.8f, -0.75f, 0f);
        }

        Instantiate(poisonCloudPrefab, spawnPos, Quaternion.identity);

        Debug.Log("독안개 생성");
    }
    private BossSkill GetAvailableSkill(float distance)
    {
        bool canRed = distance <= redSlamRange && lastSkill != BossSkill.RedSlam;
        bool canBlue = distance <= blueChargeRange && lastSkill != BossSkill.BlueCharge;
        bool canPurple = distance <= purpleSummonRange && lastSkill != BossSkill.PurpleSummon;
        bool canGreen = distance <= poisonRange && lastSkill != BossSkill.GreenPoison;

        if (canGreen && Random.value < 0.25f)
        {
            return BossSkill.GreenPoison;
        }

        if (canPurple && Random.value < 0.25f)
        {
            return BossSkill.PurpleSummon;
        }

        if (canRed && canBlue)
        {
            return Random.value < 0.5f ? BossSkill.RedSlam : BossSkill.BlueCharge;
        }

        if (canRed) return BossSkill.RedSlam;
        if (canBlue) return BossSkill.BlueCharge;
        if (canPurple) return BossSkill.PurpleSummon;
        if (canGreen) return BossSkill.GreenPoison;

        return BossSkill.None;
    }

    private void PrepareSkill(BossSkill skill)
    {
        currentSkill = skill;
        lastSkill = skill;
        hasHit = false;
        hasSummoned = false;
        hasPoisoned = false;

        if (skill == BossSkill.BlueCharge)
        {
            auraIndicator?.SetAuraColor(new Color(0.1f, 0.35f, 1f, 1f));
            chargeWarningLine?.PrepareBlueSkill(facingDirection);
            Debug.Log("보스 스킬 예고: 파랑 돌진");
        }
        else if (skill == BossSkill.RedSlam)
        {
            auraIndicator?.SetAuraColor(new Color(1f, 0.1f, 0.1f, 1f));
            chargeWarningLine?.ClearSkill();
            Debug.Log("보스 스킬 예고: 빨강 내려찍기");
        }
        else if (skill == BossSkill.PurpleSummon)
        {
            auraIndicator?.SetAuraColor(new Color(0.55f, 0.1f, 1f, 1f));
            chargeWarningLine?.ClearSkill();
            Debug.Log("보스 스킬 예고: 보라 고블린 소환");
        }
        else if (skill == BossSkill.GreenPoison)
        {
            auraIndicator?.SetAuraColor(new Color(0.1f, 1f, 0.25f, 1f));
            chargeWarningLine?.ClearSkill();
            Debug.Log("보스 스킬 예고: 초록 독안개");
        }
        ChangeState(BossState.SkillReady);
    }

    private void UpdateSkillReady()
    {
        rb.linearVelocity = Vector2.zero;

        if (stateTimer >= skillReadyTime)
        {
            if (currentSkill == BossSkill.BlueCharge)
            {
                ChangeState(BossState.BlueCharge);
            }
            else if (currentSkill == BossSkill.RedSlam)
            {
                ChangeState(BossState.RedSlam);
            }
            else if (currentSkill == BossSkill.PurpleSummon)
            {
                ChangeState(BossState.PurpleSummon);
            }
            else if (currentSkill == BossSkill.GreenPoison)
            {
                ChangeState(BossState.GreenPoison);
            }
        }
    }

    private void UpdateBlueCharge()
    {
        rb.linearVelocity = new Vector2(facingDirection * chargeSpeed, rb.linearVelocity.y);

        if (!hasHit)
        {
            CheckHit(chargeDamage, transform.position, chargeHitBoxSize);
        }

        if (stateTimer >= chargeTime)
        {
            ChangeState(BossState.Recovery);
        }
    }

    private void UpdateRedSlam()
    {
        rb.linearVelocity = Vector2.zero;

        if (!hasHit)
        {
            int direction = GetDirectionToTarget();
            StartCoroutine(SpawnRedSlamSpikesRoutine(direction));
            hasHit = true;
        }

        if (stateTimer >= 0.8f)
        {
            ChangeState(BossState.Recovery);
        }
    }

    private void UpdatePurpleSummon()
    {
        rb.linearVelocity = Vector2.zero;

        if (!hasSummoned)
        {
            SummonGoblin();
            hasSummoned = true;
        }

        if (stateTimer >= summonRecoveryTime)
        {
            ChangeState(BossState.Recovery);
        }
    }

    private void SummonGoblin()
    {
        if (goblinPrefab == null)
        {
            Debug.LogWarning("Goblin Prefab이 비어있음");
            return;
        }

        Vector3 spawnPos;

        if (summonPoint != null)
        {
            spawnPos = summonPoint.position;
        }
        else
        {
            spawnPos = transform.position + new Vector3(facingDirection * 1.5f, -0.5f, 0f);
        }

        GameObject goblin = Instantiate(goblinPrefab, spawnPos, Quaternion.identity);
        goblin.name = "EN_Goblin_Summoned";

        Debug.Log("보스 소환: 고블린 1마리");
    }

    private void UpdateRecovery()
    {
        rb.linearVelocity = Vector2.zero;

        if (stateTimer >= recoveryTime)
        {
            ChangeState(BossState.Approach);
        }
    }

    private void ChangeState(BossState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        hasHit = false;

        if (newState == BossState.Recovery || newState == BossState.Approach || newState == BossState.Idle)
        {
            chargeWarningLine?.ClearSkill();
        }
    }

    private int GetDirectionToTarget()
    {
        if (target == null) return facingDirection;

        return target.position.x >= transform.position.x ? 1 : -1;
    }

    private IEnumerator SpawnRedSlamSpikesRoutine(int direction)
    {
        if (redSlamSpikePrefab == null)
        {
            Debug.LogWarning("RedSlamSpike Prefab이 비어있음");
            yield break;
        }

        for (int i = 0; i < redSpikeCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                direction * (redSpikeStartOffset.x + redSpikeSpacing * i),
                redSpikeStartOffset.y,
                0f
            );

            GameObject spike = Instantiate(redSlamSpikePrefab, spawnPos, Quaternion.identity);

            float randomScale = Random.Range(0.9f, 1.15f);
            spike.transform.localScale *= randomScale;

            yield return new WaitForSeconds(redSpikeInterval);
        }

        Debug.Log("빨강 슬램 스파이크 순차 생성 완료");
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(transform.position, target.position);
    }

    private void UpdateFacing()
    {
        float diff = target.position.x - transform.position.x;

        if (diff > 0.1f)
        {
            facingDirection = 1;
            FaceRight();
        }
        else if (diff < -0.1f)
        {
            facingDirection = -1;
            FaceLeft();
        }
    }

    private void CheckHit(int damage, Vector2 center, Vector2 size)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, playerLayer);

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
                hasHit = true;
            }
        }
    }

    private void FaceRight()
    {
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, chargeHitBoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + slamHitBoxOffset, slamHitBoxSize);
    }
}