using UnityEngine;

public class FocusChargeBar : MonoBehaviour
{
    [SerializeField] private Transform fill;

    private SpriteRenderer[] renderers;
    private Vector3 originalFillScale;
    private Vector3 originalFillPosition;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);

        if (fill != null)
        {
            originalFillScale = fill.localScale;
            originalFillPosition = fill.localPosition;
        }

        Hide();
    }

    public void Show()
    {
        SetVisible(true);
        SetProgress(0f);
    }

    public void Hide()
    {
        SetVisible(false);
        SetProgress(0f);
    }

    public void SetProgress(float progress)
    {
        if (fill == null) return;

        progress = Mathf.Clamp01(progress);

        Vector3 scale = originalFillScale;
        scale.x = originalFillScale.x * progress;
        fill.localScale = scale;

        Vector3 pos = originalFillPosition;
        pos.x = originalFillPosition.x - originalFillScale.x * (1f - progress) * 0.5f;
        fill.localPosition = pos;
    }

    private void SetVisible(bool visible)
    {
        if (renderers == null) return;

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;

            Color c = sr.color;
            c.a = visible ? 1f : 0f;
            sr.color = c;
        }
    }
}