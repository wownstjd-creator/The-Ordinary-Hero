using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFocus : MonoBehaviour
{
    [Header("Focus")]
    [SerializeField] private float focusDuration = 3f;

    [Header("Detection")]
    [SerializeField] private float senseRadius = 4f;
    [SerializeField] private LayerMask detectableLayer;

    [Header("Pulse")]
    [SerializeField] private SensePulse pulsePrefab;

    [SerializeField] private float focusHoldAfterComplete = 0.6f;

    private float focusVisualHoldTimer;

    public bool IsFocusVisualActive => isFocusing || focusVisualHoldTimer > 0f;

    private float focusTimer;
    private bool isFocusing;
    private SensePulse activePulse;

    private readonly HashSet<DetectableObject> detectedThisFocus = new();

    public bool IsFocusing => isFocusing;
    public float FocusProgress => focusTimer / focusDuration;

    public void OnSense(InputValue value)
    {
        if (value.isPressed)
        {
            StartFocus();
        }
    }

    private void Update()
    {
        if (focusVisualHoldTimer > 0f)
        {
            focusVisualHoldTimer -= Time.deltaTime;
        }

        if (!isFocusing) return;

        focusTimer += Time.deltaTime;

        float currentRadius = Mathf.Lerp(0f, senseRadius, FocusProgress);

        if (activePulse != null)
        {
            activePulse.SetRadius(currentRadius);
        }

        DetectObjectsByRadius(currentRadius);

        if (focusTimer >= focusDuration)
        {
            CompleteFocus();
        }
    }

    private void StartFocus()
    {
        if (isFocusing) return;

        isFocusing = true;
        focusTimer = 0f;
        detectedThisFocus.Clear();

        if (pulsePrefab != null)
        {
            activePulse = Instantiate(pulsePrefab, transform.position, Quaternion.identity);
            activePulse.transform.SetParent(transform);
            activePulse.transform.localPosition = Vector3.zero;
            activePulse.SetRadius(0f);
        }

        Debug.Log("집중 시작");
    }

    private void CompleteFocus()
    {
        isFocusing = false;
        focusTimer = 0f;

        if (activePulse != null)
        {
            Destroy(activePulse.gameObject);
        }

        Debug.Log("감지 완료");
        focusVisualHoldTimer = focusHoldAfterComplete;
    }

    public void InterruptFocus()
    {
        if (!isFocusing) return;

        isFocusing = false;
        focusTimer = 0f;
        detectedThisFocus.Clear();

        if (activePulse != null)
        {
            Destroy(activePulse.gameObject);
        }

        Debug.Log("집중이 끊김");
    }

    private void DetectObjectsByRadius(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            detectableLayer
        );

        foreach (Collider2D hit in hits)
        {
            DetectableObject detectable = hit.GetComponent<DetectableObject>();

            if (detectable == null)
            {
                detectable = hit.GetComponentInParent<DetectableObject>();
            }

            if (detectable == null) continue;

            if (detectedThisFocus.Contains(detectable)) continue;

            detectable.Reveal();
            detectedThisFocus.Add(detectable);

            Debug.Log("파동 감지: " + detectable.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, senseRadius);
    }
}