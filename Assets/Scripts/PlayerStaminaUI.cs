using TMPro;
using UnityEngine;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private TMP_Text staminaText;

    private void Awake()
    {
        if (staminaText == null)
        {
            staminaText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged += UpdateStaminaText;
            UpdateStaminaText(playerStamina.CurrentStamina, playerStamina.MaxStamina);
        }
    }

    private void OnDisable()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged -= UpdateStaminaText;
        }
    }

    private void UpdateStaminaText(float current, float max)
    {
        staminaText.text = $"ST: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}