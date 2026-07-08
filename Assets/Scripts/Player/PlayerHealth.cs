using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float deathDelay = 1.2f;
    [SerializeField] private Transform respawnPoint;

    [Header("Hit")]
    [SerializeField] private float hitLockTime = 0.25f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int currentHp;
    private bool isDead;
    private bool isHitLocked;
    private float hitLockTimer;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    public bool IsDead => isDead;
    public bool IsHitLocked => isHitLocked;
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHp, maxHp);
    }

    private void Update()
    {
        if (!isHitLocked) return;

        hitLockTimer -= Time.deltaTime;

        if (hitLockTimer <= 0f)
        {
            isHitLocked = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (playerMovement != null && playerMovement.IsDashing)
        {
            return;
        }

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHealthChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        HitReaction();
    }

    private void HitReaction()
    {
        isHitLocked = true;
        hitLockTimer = hitLockTime;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("3_Damaged");
        }
    }

    private void Die()
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("4_Death");
        }

        Invoke(nameof(Respawn), deathDelay);
    }

    private void Respawn()
    {
        currentHp = maxHp;
        isDead = false;
        isHitLocked = false;

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        OnHealthChanged?.Invoke(currentHp, maxHp);
    }
}