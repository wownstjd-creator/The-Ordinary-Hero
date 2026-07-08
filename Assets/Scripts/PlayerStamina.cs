using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float recoverSpeed = 10f;

    [SerializeField] private StaminaWarning staminaWarning;

    [Header("Cost")]
    [SerializeField] private float dashCost = 75f;
    [SerializeField] private float jumpCost = 25f;
    [SerializeField] private float focusCost = 50f;

    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public event Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Start()
    {
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    private void Update()
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += recoverSpeed * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public bool TryUseDashStamina()
    {
        return TryUseStamina(dashCost);
    }

    public bool TryUseJumpStamina()
    {
        return TryUseStamina(jumpCost);
    }

    public bool TryUseFocusStamina()
    {
        return TryUseStamina(focusCost);
    }

    private bool TryUseStamina(float cost)
    {
        if (currentStamina < cost)
        {
            if (staminaWarning != null)
            {
                staminaWarning.Show();
            }

            return false;
        }

        currentStamina -= cost;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        return true;
    }
}