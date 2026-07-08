using UnityEngine;

public class BossWakeController : MonoBehaviour
{
    private BossGoblinKingAI bossAI;
    private BossAuraIndicator auraIndicator;

    private Collider2D[] bossColliders;
    private Collider2D[] playerColliders;
    private BossChargeWarningLine chargeWarningLine;
    private bool isAwake;

    private void Awake()
    {
        bossAI = GetComponent<BossGoblinKingAI>();
        auraIndicator = GetComponentInChildren<BossAuraIndicator>(true);

        bossColliders = GetComponentsInChildren<Collider2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerColliders = player.GetComponentsInChildren<Collider2D>();
            IgnorePlayerCollision(true);
        }
        chargeWarningLine = GetComponentInChildren<BossChargeWarningLine>(true);
    }

    public void WakeUp()
    {
        if (isAwake) return;

        isAwake = true;

        IgnorePlayerCollision(true);

        if (auraIndicator != null)
        {
            auraIndicator.Unlock();
        }

        if (bossAI != null)
        {
            bossAI.WakeUp();
        }
        if (chargeWarningLine != null)
        {
            chargeWarningLine.Unlock();
        }

        Debug.Log("보스 기상");
    }

    private void IgnorePlayerCollision(bool ignore)
    {
        if (bossColliders == null || playerColliders == null) return;

        foreach (Collider2D bossCol in bossColliders)
        {
            if (bossCol == null) continue;
            if (bossCol.isTrigger) continue;

            foreach (Collider2D playerCol in playerColliders)
            {
                if (playerCol == null) continue;
                if (playerCol.isTrigger) continue;

                Physics2D.IgnoreCollision(bossCol, playerCol, ignore);
            }
        }
    }
}