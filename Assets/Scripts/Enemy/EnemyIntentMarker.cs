using UnityEngine;

public class EnemyIntentMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GoblinStateMachine stateMachine;
    [SerializeField] private SpriteRenderer markerRenderer;

    [Header("Colors")]
    [SerializeField] private Color chaseColor = Color.blue;
    [SerializeField] private Color attackReadyColor = Color.red;
    [SerializeField] private Color attackColor = new Color(0.6f, 0f, 0f, 1f);
    [SerializeField] private Color recoveryColor = Color.yellow;

    [Header("Reveal")]
    [SerializeField] private float revealDuration = 1.2f;
    [SerializeField] private float blinkSpeed = 8f;

    private float revealTimer;

    private void Awake()
    {
        if (stateMachine == null)
        {
            stateMachine = GetComponent<GoblinStateMachine>();
        }

        Hide();
    }

    private void Update()
    {
        if (markerRenderer == null) return;

        if (revealTimer > 0f)
        {
            revealTimer -= Time.deltaTime;
            UpdateMarker();
        }
        else
        {
            Hide();
        }
    }

    public void RevealIntent()
    {
        revealTimer = revealDuration;
    }

    private void UpdateMarker()
    {
        if (stateMachine == null) return;

        EnemyState state = stateMachine.CurrentState;

        switch (state)
        {
            case EnemyState.Chase:
                Show(chaseColor);
                break;

            case EnemyState.AttackReady:
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
                Color blinkColor = attackReadyColor;
                blinkColor.a = Mathf.Lerp(0.25f, 1f, alpha);
                Show(blinkColor);
                break;

            case EnemyState.Attack:
                Show(attackColor);
                break;

            case EnemyState.Recovery:
                Show(recoveryColor);
                break;
            
            case EnemyState.Stagger:
                Show(Color.white);
                break;

            default:
                Hide();
                break;
        }
    }

    private void Show(Color color)
    {
        markerRenderer.enabled = true;
        markerRenderer.color = color;
    }

    private void Hide()
    {
        if (markerRenderer != null)
        {
            markerRenderer.enabled = false;
        }
    }
}