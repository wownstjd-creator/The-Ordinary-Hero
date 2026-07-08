using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private bool requireUnlocked = true;
    [SerializeField] private SenseRevealObject revealObject;

    private void Awake()
    {
        if (revealObject == null)
        {
            revealObject = GetComponent<SenseRevealObject>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (requireUnlocked && revealObject != null && !revealObject.IsUnlocked)
        {
            Debug.Log("포탈이 아직 감지되지 않음");
            return;
        }

        Debug.Log("맵 클리어! 다음 맵으로 이동 예정");
    }
}