using TMPro;
using UnityEngine;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {
        if (hpText == null)
        {
            hpText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHpText;
            UpdateHpText(playerHealth.CurrentHp, playerHealth.MaxHp);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHpText;
        }
    }

    private void UpdateHpText(int currentHp, int maxHp)
    {
        hpText.text = $"HP: {currentHp} / {maxHp}";
    }
}