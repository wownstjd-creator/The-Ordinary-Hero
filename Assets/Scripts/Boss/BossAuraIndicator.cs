using UnityEngine;

public class BossAuraIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer auraRenderer;
    [SerializeField] private float visibleAlpha = 0.35f;

    private Color currentColor = Color.clear;
    private bool isUnlocked;

    private void Awake()
    {
        if (auraRenderer == null)
        {
            auraRenderer = GetComponent<SpriteRenderer>();
        }

        Hide();
    }

    public void Unlock()
    {
        isUnlocked = true;
        Hide();
    }

    public void SetAuraColor(Color color)
    {
        currentColor = color;
        currentColor.a = visibleAlpha;

        // 이미 감지로 보이는 중이면 즉시 색 변경
        if (auraRenderer != null && auraRenderer.enabled)
        {
            auraRenderer.color = currentColor;
        }
    }

    public void Reveal()
    {
        if (!isUnlocked) return;
        if (auraRenderer == null) return;

        auraRenderer.enabled = true;
        auraRenderer.color = currentColor;

        Debug.Log("오라 표시 색상: " + currentColor);
    }

    public void Hide()
    {
        if (auraRenderer == null) return;

        auraRenderer.enabled = false;
    }
}