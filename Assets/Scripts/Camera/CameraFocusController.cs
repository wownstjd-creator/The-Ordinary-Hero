using System;
using System.Collections;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    public static CameraFocusController Instance { get; private set; }

    [Header("Target")]
    [SerializeField] private Transform playerTarget;

    [Header("Timing")]
    [SerializeField] private float moveTime = 1.2f;
    [SerializeField] private float holdTime = 1.5f;

    private CameraFollow cameraFollow;
    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
        cameraFollow = GetComponent<CameraFollow>();
    }

    public void FocusThenReturn(Transform focusTarget, Action onArrive = null)
    {
        if (focusTarget == null) return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(FocusRoutine(focusTarget, onArrive));
    }

    private IEnumerator FocusRoutine(Transform focusTarget, Action onArrive)
    {
        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }

        Vector3 startPos = transform.position;
        Vector3 focusPos = focusTarget.position;
        focusPos.z = transform.position.z;

        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            float t = timer / moveTime;
            transform.position = Vector3.Lerp(startPos, focusPos, t);
            yield return null;
        }

        onArrive?.Invoke();

        yield return new WaitForSeconds(holdTime);

        Vector3 returnStartPos = transform.position;
        Vector3 returnPos = playerTarget.position;
        returnPos.z = transform.position.z;

        timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            float t = timer / moveTime;
            transform.position = Vector3.Lerp(returnStartPos, returnPos, t);
            yield return null;
        }

        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }
    }
}