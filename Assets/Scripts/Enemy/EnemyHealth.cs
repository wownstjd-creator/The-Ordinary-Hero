using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float hitRevealTime = 0.45f;
    [SerializeField] private float deathDestroyDelay = 0.7f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int currentHp;
    private DetectableObject detectable;
    private bool isDead;

    private void Awake()
    {
        currentHp = maxHp;
        detectable = GetComponent<DetectableObject>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (detectable != null)
        {
            detectable.RevealFullTemporary(hitRevealTime);
        }

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("3_Damaged");
        }

        Debug.Log($"{gameObject.name} 피격! 남은 HP: {currentHp}");
    }

    private void Die()
    {
        isDead = true;

        if (detectable != null)
        {
            detectable.RevealFullTemporary(deathDestroyDelay);
        }

        if (animator != null)
        {
            animator.SetTrigger("4_Death");
        }

        Debug.Log($"{gameObject.name} 사망");

        Destroy(gameObject, deathDestroyDelay);
    }
}