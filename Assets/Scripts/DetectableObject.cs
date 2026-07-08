using UnityEngine;

public class DetectableObject : MonoBehaviour
{
    private enum RevealMode
    {
        Hidden,
        Presence,
        Silhouette,
        Full
    }
    [Header("Hit Flash")]
    [SerializeField] private float flashTime = 0.06f;

    private float flashTimer;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer presenceMarker;
    [SerializeField] private SpriteRenderer intentMarkerRenderer;

    [Header("Visibility")]
    [SerializeField] private float hiddenAlpha = 0f;
    [SerializeField] private float presenceAlpha = 0.85f;
    [SerializeField] private float silhouetteAlpha = 0.9f;
    [SerializeField] private float fullAlpha = 1f;
    [SerializeField] private float revealTime = 1.2f;

    [Header("Memory")]
    [SerializeField] private int familiarity = 0;
    [SerializeField] private float memoryAlphaPerFamiliarity = 0.025f;
    [SerializeField] private float maxMemoryAlpha = 0.12f;

    private SpriteRenderer[] bodyRenderers;
    private Color[] originalColors;
    private EnemyIntentMarker intentMarker;
    private float revealTimer;
    private RevealMode currentMode = RevealMode.Hidden;

    private void Awake()
    {
        bodyRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (presenceMarker != null)
        {
            bodyRenderers = RemovePresenceMarker(bodyRenderers);
        }
        if (intentMarkerRenderer != null)
        {
            bodyRenderers = RemoveSpecificRenderer(bodyRenderers, intentMarkerRenderer);
        }

        originalColors = new Color[bodyRenderers.Length];

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            originalColors[i] = bodyRenderers[i].color;
        }

        ApplyHidden();
        intentMarker = GetComponent<EnemyIntentMarker>();
    }

    private void Update()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            ApplyFlash();
            return;
        }
        if (revealTimer > 0f)
        {
            revealTimer -= Time.deltaTime;

            switch (currentMode)
            {
                case RevealMode.Presence:
                    ApplyPresence();
                    break;

                case RevealMode.Silhouette:
                    ApplySilhouette(silhouetteAlpha);
                    break;

                case RevealMode.Full:
                    ApplyOriginalColor(fullAlpha);
                    break;
            }

            return;
        }

        ApplyMemory();
    }
    public void HitFlash()
    {
        flashTimer = flashTime;
    }

    private void ApplyFlash()
    {
        SetPresenceAlpha(0f);

        foreach (SpriteRenderer sr in bodyRenderers)
        {
            if (sr == null) continue;

            Color color = Color.white;
            color.a = 1f;
            sr.color = color;
        }
    }
    public void Reveal()
    {
        familiarity++;
        revealTimer = revealTime;

        if (familiarity == 1)
        {
            currentMode = RevealMode.Presence;
        }
        else if (familiarity == 2)
        {
            currentMode = RevealMode.Silhouette;
        }
        else
        {
            currentMode = RevealMode.Full;
        }

        if (intentMarker != null && familiarity >= 2)
        {
            intentMarker.RevealIntent();
        }
    }

    public void RevealFullTemporary(float duration)
    {
        revealTimer = duration;
        currentMode = RevealMode.Full;
        ApplyOriginalColor(1f);
    }

    private void ApplyHidden()
    {
        SetBodyAlpha(hiddenAlpha);
        SetPresenceAlpha(hiddenAlpha);
    }

    private void ApplyPresence()
    {
        SetBodyAlpha(hiddenAlpha);
        SetPresenceAlpha(presenceAlpha);
    }

    private void ApplyMemory()
    {
        float memoryAlpha = Mathf.Min(
            familiarity * memoryAlphaPerFamiliarity,
            maxMemoryAlpha
        );

        if (familiarity <= 0 || memoryAlpha <= 0.01f)
        {
            ApplyHidden();
            return;
        }

        if (familiarity == 1)
        {
            SetBodyAlpha(hiddenAlpha);
            SetPresenceAlpha(memoryAlpha);
        }
        else
        {
            SetPresenceAlpha(hiddenAlpha);
            ApplySilhouette(memoryAlpha);
        }
    }

    private void ApplySilhouette(float alpha)
    {
        SetPresenceAlpha(hiddenAlpha);

        foreach (SpriteRenderer sr in bodyRenderers)
        {
            if (sr == null) continue;
            sr.color = new Color(0f, 0f, 0f, alpha);
        }
    }

    private void ApplyOriginalColor(float alpha)
    {
        SetPresenceAlpha(hiddenAlpha);

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            if (bodyRenderers[i] == null) continue;

            Color color = originalColors[i];
            color.a = alpha;
            bodyRenderers[i].color = color;
        }
    }

    private void SetBodyAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in bodyRenderers)
        {
            if (sr == null) continue;

            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
        }
    }

    private void SetPresenceAlpha(float alpha)
    {
        if (presenceMarker == null) return;

        Color color = presenceMarker.color;
        color.a = alpha;
        presenceMarker.color = color;
    }

    private SpriteRenderer[] RemovePresenceMarker(SpriteRenderer[] source)
    {
        int count = 0;

        foreach (SpriteRenderer sr in source)
        {
            if (sr != presenceMarker)
            {
                count++;
            }
        }

        SpriteRenderer[] result = new SpriteRenderer[count];
        int index = 0;

        foreach (SpriteRenderer sr in source)
        {
            if (sr != presenceMarker)
            {
                result[index] = sr;
                index++;
            }
        }

        return result;
    }
    private SpriteRenderer[] RemoveSpecificRenderer(SpriteRenderer[] source, SpriteRenderer targetRenderer)
    {
        int count = 0;

        foreach (SpriteRenderer sr in source)
        {
            if (sr != targetRenderer)
            {
                count++;
            }
        }

        SpriteRenderer[] result = new SpriteRenderer[count];
        int index = 0;

        foreach (SpriteRenderer sr in source)
        {
            if (sr != targetRenderer)
            {
                result[index] = sr;
                index++;
            }
        }

        return result;
    }
}