using UnityEngine;

public class BossChargeWarningLine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lineRenderer;
    [SerializeField] private float visibleAlpha = 0.35f;

    private bool isUnlocked;
    private bool isBlueSkillPrepared;
    private bool isSenseVisible;

    private float defaultLocalX;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<SpriteRenderer>();
        }

        defaultLocalX = Mathf.Abs(transform.localPosition.x);

        HideRenderer();
    }

    public void Unlock()
    {
        isUnlocked = true;
        HideRenderer();
    }

    public void PrepareBlueSkill(int direction)
    {
        isBlueSkillPrepared = true;

        Vector3 pos = transform.localPosition;
        pos.x = defaultLocalX * direction;
        transform.localPosition = pos;

        RefreshVisibleState();
    }

    public void ClearSkill()
    {
        isBlueSkillPrepared = false;
        RefreshVisibleState();
    }

    public void SetSenseVisible(bool visible)
    {
        isSenseVisible = visible;
        RefreshVisibleState();
    }

    private void RefreshVisibleState()
    {
        if (!isUnlocked || !isBlueSkillPrepared || !isSenseVisible)
        {
            HideRenderer();
            return;
        }

        ShowRenderer();
    }

    private void ShowRenderer()
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;

        Color color = Color.blue;
        color.a = visibleAlpha;
        lineRenderer.color = color;
    }

    private void HideRenderer()
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = false;
    }
}