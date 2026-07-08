using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private bool isStopping;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Stop(float duration)
    {
        if (!gameObject.activeInHierarchy) return;

        if (isStopping)
        {
            return;
        }

        Debug.Log($"히트스톱 시작: {duration}초");
        StartCoroutine(StopRoutine(duration));
    }

    private IEnumerator StopRoutine(float duration)
    {
        isStopping = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isStopping = false;

        Debug.Log("히트스톱 종료");
    }
}