using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;

    [Header("Arena Bounds")]
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    [Header("Boss")]
    [SerializeField] private BossGoblinKingAI bossAI;

    private bool isArenaActive;

    public float LeftX => leftLimit != null ? leftLimit.position.x : -999f;
    public float RightX => rightLimit != null ? rightLimit.position.x : 999f;
    public bool IsArenaActive => isArenaActive;

    private void Awake()
    {
        SetWalls(false);

        if (bossAI == null)
        {
            bossAI = FindFirstObjectByType<BossGoblinKingAI>();
        }

        if (bossAI != null)
        {
            bossAI.SetArena(this);
        }
    }

    public void StartBossArena()
    {
        if (isArenaActive) return;

        isArenaActive = true;
        SetWalls(true);

        Debug.Log("보스 아레나 시작: 벽 활성화");
    }

    public void EndBossArena()
    {
        isArenaActive = false;
        SetWalls(false);

        Debug.Log("보스 아레나 종료: 벽 비활성화");
    }

    private void SetWalls(bool active)
    {
        if (leftWall != null)
        {
            leftWall.SetActive(active);
        }

        if (rightWall != null)
        {
            rightWall.SetActive(active);
        }
    }
}