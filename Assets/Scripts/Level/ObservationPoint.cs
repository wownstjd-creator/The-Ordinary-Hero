using UnityEngine;

public class ObservationPoint : MonoBehaviour
{
    [Header("Reveal Targets")]
    [SerializeField] private SenseRevealObject[] revealTargets;

    [Header("Camera")]
    [SerializeField] private Transform cameraFocusTarget;

    [Header("Boss")]
    [SerializeField] private BossWakeController bossWakeController;

    private bool playerInside;

    public bool CanActivate => playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        Debug.Log("OBS 진입");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        Debug.Log("OBS 이탈");
    }

    public void Activate()
    {
        Debug.Log("OBS 활성화: 보스 위치 공개");

        foreach (SenseRevealObject target in revealTargets)
        {
            if (target == null) continue;

            target.ForceRevealAndUnlock();
            Debug.Log("해금 대상: " + target.name);
        }

        if (CameraFocusController.Instance != null)
        {
            CameraFocusController.Instance.FocusThenReturn(
                cameraFocusTarget,
                OnCameraReachedBoss
            );
        }
    }

    private void OnCameraReachedBoss()
    {
        if (bossWakeController != null)
        {
            bossWakeController.WakeUp();
        }
    }
}