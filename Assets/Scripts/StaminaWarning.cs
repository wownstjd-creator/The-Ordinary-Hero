using TMPro;
using UnityEngine;
using System.Collections;

public class StaminaWarning : MonoBehaviour
{
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private float showTime = 0.8f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (warningText == null)
        {
            warningText = GetComponent<TMP_Text>();
        }

        warningText.enabled = false;
    }

    public void Show()
    {
        if (warningText == null) return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        warningText.enabled = true;

        yield return new WaitForSeconds(showTime);

        warningText.enabled = false;
    }
}