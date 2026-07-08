using UnityEngine;
using System.Collections;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float hitRevealTime = 0.45f;
    [SerializeField] private float deathDestroyDelay = 0.7f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Hit Flash")]
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    [SerializeField] private float flashTime = 0.06f;

     
    private int currentHp;
    private DetectableObject detectable;
    private GoblinStateMachine stateMachine;
    private bool isDead;

    private void Awake()
    {
        currentHp = maxHp;
        detectable = GetComponent<DetectableObject>();
        stateMachine = GetComponent<GoblinStateMachine>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int finalDamage = damage;

        if (stateMachine != null && stateMachine.IsStaggered)
        {
            finalDamage *= 2;
            Debug.Log("스태거 추가 데미지!");
        }

        currentHp -= finalDamage;
        StartCoroutine(HitFlash());

        if (detectable != null)
        {
            detectable.HitFlash();
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

        Debug.Log($"{gameObject.name} 피격! 데미지: {finalDamage}, 남은 HP: {currentHp}");
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
    private IEnumerator HitFlash()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.white;
        }

        yield return new WaitForSeconds(flashTime);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
    }
}