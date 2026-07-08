using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFocus : MonoBehaviour
{
    [Header("Focus")]
    [SerializeField] private float focusDuration = 3f;
    [SerializeField] private float focusHoldAfterComplete = 0.6f;

    [Header("Sense Level")]
    [SerializeField] private int currentSenseLevel = 1;

    [Header("Auto Intro")]
    [SerializeField] private bool autoFocusOnStart = true;
    [SerializeField] private float autoFocusDelay = 0.8f;

    [Header("Detection")]
    [SerializeField] private float senseRadius = 4f;
    [SerializeField] private LayerMask detectableLayer;

    [Header("Pulse")]
    [SerializeField] private SensePulse pulsePrefab;

    [Header("UI")]
    [SerializeField] private FocusChargeBar focusChargeBar;

    private float focusTimer;
    private float focusVisualHoldTimer;
    private bool isFocusing;

    private SensePulse activePulse;
    private PlayerStamina playerStamina;
    private ObservationPoint currentObservationPoint;

    private readonly HashSet<DetectableObject> detectedThisFocus = new();

    public bool IsFocusing => isFocusing;
    public bool IsFocusVisualActive => isFocusing || focusVisualHoldTimer > 0f;
    public float FocusProgress => focusDuration <= 0f ? 1f : focusTimer / focusDuration;

    private void Awake()
    {
        playerStamina = GetComponent<PlayerStamina>();
    }

    private void Start()
    {
        if (autoFocusOnStart)
        {
            Invoke(nameof(StartAutoFocus), autoFocusDelay);
        }
    }

    public void OnSense(InputValue value)
    {
        if (value.isPressed)
        {
            StartFocus(true);
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

        if (focusChargeBar != null)
        {
            focusChargeBar.SetProgress(FocusProgress);
        }

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

    private void StartAutoFocus()
    {
        StartFocus(false);
    }

    private void StartFocus(bool useStamina)
    {
        if (isFocusing) return;

        if (useStamina && playerStamina != null && !playerStamina.TryUseFocusStamina())
        {
            return;
        }

        isFocusing = true;
        focusTimer = 0f;
        detectedThisFocus.Clear();

        RevealSenseObjects();
        RevealBossSenseVisuals();

        if (currentObservationPoint != null && currentObservationPoint.CanActivate)
        {
            currentObservationPoint.Activate();
        }

        if (focusChargeBar != null)
        {
            focusChargeBar.Show();
        }

        if (pulsePrefab != null)
        {
            activePulse = Instantiate(pulsePrefab, transform.position, Quaternion.identity);
            activePulse.transform.SetParent(transform);
            activePulse.transform.localPosition = Vector3.zero;
            activePulse.SetRadius(0f);
        }
    }

    private void CompleteFocus()
    {
        isFocusing = false;
        focusTimer = 0f;
        focusVisualHoldTimer = focusHoldAfterComplete;

        if (focusChargeBar != null)
        {
            focusChargeBar.Hide();
        }

        if (activePulse != null)
        {
            Destroy(activePulse.gameObject);
            activePulse = null;
        }

        HideTemporarySenseObjects();
        HideBossSenseVisuals();
    }

    public void InterruptFocus()
    {
        if (!isFocusing) return;

        isFocusing = false;
        focusTimer = 0f;
        detectedThisFocus.Clear();

        if (focusChargeBar != null)
        {
            focusChargeBar.Hide();
        }

        if (activePulse != null)
        {
            Destroy(activePulse.gameObject);
            activePulse = null;
        }

        HideTemporarySenseObjects();
        HideBossSenseVisuals();
    }

    private void RevealSenseObjects()
    {
        SenseRevealObject[] objects = FindObjectsByType<SenseRevealObject>(FindObjectsSortMode.None);

        foreach (SenseRevealObject obj in objects)
        {
            obj.TryReveal(currentSenseLevel);
        }
    }

    private void HideTemporarySenseObjects()
    {
        SenseRevealObject[] objects = FindObjectsByType<SenseRevealObject>(FindObjectsSortMode.None);

        foreach (SenseRevealObject obj in objects)
        {
            obj.HideIfTemporary();
        }
    }

    private void RevealBossSenseVisuals()
    {
        BossAuraIndicator[] auras = FindObjectsByType<BossAuraIndicator>(FindObjectsSortMode.None);

        foreach (BossAuraIndicator aura in auras)
        {
            aura.Reveal();
        }

        BossChargeWarningLine[] lines = FindObjectsByType<BossChargeWarningLine>(FindObjectsSortMode.None);

        foreach (BossChargeWarningLine line in lines)
        {
            line.SetSenseVisible(true);
        }
    }

    private void HideBossSenseVisuals()
    {
        BossAuraIndicator[] auras = FindObjectsByType<BossAuraIndicator>(FindObjectsSortMode.None);

        foreach (BossAuraIndicator aura in auras)
        {
            aura.Hide();
        }

        BossChargeWarningLine[] lines = FindObjectsByType<BossChargeWarningLine>(FindObjectsSortMode.None);

        foreach (BossChargeWarningLine line in lines)
        {
            line.SetSenseVisible(false);
        }
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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ObservationPoint obs = other.GetComponent<ObservationPoint>();

        if (obs != null)
        {
            currentObservationPoint = obs;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ObservationPoint obs = other.GetComponent<ObservationPoint>();

        if (obs != null && obs == currentObservationPoint)
        {
            currentObservationPoint = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, senseRadius);
    }
}