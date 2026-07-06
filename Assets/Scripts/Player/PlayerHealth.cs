using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 5;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        Debug.Log($"Hero 피격! 남은 HP: {currentHp}");

        if (currentHp <= 0)
        {
            Debug.Log("Hero 사망");
        }
    }
}