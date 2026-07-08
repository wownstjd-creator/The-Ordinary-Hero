using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private BossArenaController arenaController;

    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        if (arenaController != null)
        {
            arenaController.StartBossArena();
        }
    }
}