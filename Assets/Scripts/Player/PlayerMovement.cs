using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.15f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.7f;

    [Header("Direction")]
    [SerializeField] private bool defaultFaceLeft = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private PlayerStamina playerStamina;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private PlayerFocus playerFocus;
    private PlayerAttack playerAttack;
    private PlayerParry playerParry;
    private PlayerHealth playerHealth;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float facingDirection = 1f;

    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        playerFocus = GetComponent<PlayerFocus>();
        playerAttack = GetComponent<PlayerAttack>();
        playerParry = GetComponent<PlayerParry>();
        playerHealth = GetComponent<PlayerHealth>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        facingDirection = defaultFaceLeft ? -1f : 1f;
        playerStamina = GetComponent<PlayerStamina>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanMove()) return;
        if (!IsGrounded()) return;

        if (playerStamina != null && !playerStamina.TryUseJumpStamina())
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed) return;
        if (!CanMove()) return;
        if (!IsGrounded()) return;
        if (dashCooldownTimer > 0f) return;
        if (playerStamina != null && !playerStamina.TryUseDashStamina())
        {
            return;
        }
        StartDash();
    }

    private void Update()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }

        if (CanTurn())
        {
            UpdateFacingDirection();
        }

        bool isMoving = CanMove() && !isDashing && Mathf.Abs(moveInput.x) > 0.01f;

        if (animator != null)
        {
            animator.SetBool("1_Move", isMoving);
        }
    }

    private void FixedUpdate()
    {
        if (!CanMove())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            true
        );
        Debug.Log("대쉬");
    }

    private void EndDash()
    {
        isDashing = false;
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );
    }

    private void UpdateFacingDirection()
    {
        if (moveInput.x > 0.01f)
        {
            FaceRight();
        }
        else if (moveInput.x < -0.01f)
        {
            FaceLeft();
        }
    }

    private bool CanMove()
    {
        if (playerHealth != null && playerHealth.IsDead) return false;
        if (playerHealth != null && playerHealth.IsHitLocked) return false;
        if (playerFocus != null && playerFocus.IsFocusing) return false;
        if (playerAttack != null && playerAttack.IsAttacking) return false;
        if (playerParry != null && playerParry.IsLockedByFailedParry) return false;

        return true;
    }

    private bool CanTurn()
    {
        if (playerHealth != null && playerHealth.IsDead) return false;
        if (playerHealth != null && playerHealth.IsHitLocked) return false;
        if (playerFocus != null && playerFocus.IsFocusing) return false;
        if (playerAttack != null && playerAttack.IsAttacking) return false;
        if (playerParry != null && playerParry.IsParrying) return false;
        if (playerParry != null && playerParry.IsLockedByFailedParry) return false;

        return true;
    }

    private void FaceRight()
    {
        facingDirection = 1f;

        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FaceLeft()
    {
        facingDirection = -1f;

        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}