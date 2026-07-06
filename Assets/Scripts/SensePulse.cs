using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SensePulse : MonoBehaviour
{
    [SerializeField] private int segments = 128;
    [SerializeField] private float lineWidth = 0.06f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void SetRadius(float radius)
    {
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}