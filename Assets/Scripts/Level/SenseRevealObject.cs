using UnityEngine;

public class SenseRevealObject : MonoBehaviour
{
    [Header("Sense Level")]
    [SerializeField] private int requiredSenseLevel = 1;

    [Header("Reveal")]
    [SerializeField] private float revealedAlpha = 1f;
    [SerializeField] private float hiddenAlpha = 0f;
    [SerializeField] private bool startHidden = true;
    [SerializeField] private bool hideAfterFocus = true;

    private SpriteRenderer[] spriteRenderers;
    private bool isRevealed;
    private bool isUnlocked;

    public bool HideAfterFocus => hideAfterFocus;
    public bool IsUnlocked => isUnlocked;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        if (startHidden) Hide();
        else Reveal();
    }

    public void TryReveal(int senseLevel)
    {
        if (!isUnlocked && senseLevel < requiredSenseLevel)
        {
            return;
        }

        Reveal();
    }

    public void ForceRevealAndUnlock()
    {
        isUnlocked = true;
        Reveal();
    }

    public void Reveal()
    {
        isRevealed = true;
        SetAlpha(revealedAlpha);
    }

    public void Hide()
    {
        isRevealed = false;
        SetAlpha(hiddenAlpha);
    }

    public void HideIfTemporary()
    {
        if (!hideAfterFocus) return;
        Hide();
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr == null) continue;

            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
        }
    }
}