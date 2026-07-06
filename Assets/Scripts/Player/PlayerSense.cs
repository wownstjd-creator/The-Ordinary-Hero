using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSense : MonoBehaviour
{
    [Header("Sense")]
    [SerializeField] private float senseRadius = 4f;
    [SerializeField] private LayerMask detectableLayer;

    public void OnSense(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            senseRadius,
            detectableLayer
        );

        foreach (Collider2D hit in hits)
        {
            DetectableObject detectable = hit.GetComponent<DetectableObject>();

            if (detectable == null)
            {
                detectable = hit.GetComponentInParent<DetectableObject>();
            }

            if (detectable != null)
            {
                detectable.Reveal();
            }
        }

        Debug.Log("감지 사용: " + hits.Length + "개 발견");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, senseRadius);
    }
}