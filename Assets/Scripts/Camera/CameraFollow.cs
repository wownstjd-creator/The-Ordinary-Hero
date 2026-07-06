using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Zoom")]
    [SerializeField] private PlayerFocus playerFocus;
    [SerializeField] private float normalSize = 5f;
    [SerializeField] private float focusSize = 3.2f;
    [SerializeField] private float zoomSpeed = 3f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = normalSize;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
        }

        float targetSize = normalSize;

        if (playerFocus != null && playerFocus.IsFocusVisualActive)
        {
            targetSize = focusSize;
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            zoomSpeed * Time.deltaTime
        );
    }
}