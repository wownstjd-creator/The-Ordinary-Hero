using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Direction")]
    [SerializeField] private bool defaultFaceLeft = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerFocus playerFocus;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerFocus = GetComponent<PlayerFocus>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (playerAttack != null && playerAttack.IsAttacking)
        {
            return;
        }
        if(playerFocus != null && playerFocus.IsFocusing)
            return;

        if (moveInput.x > 0.01f)
        {
            FaceRight();
        }
        else if (moveInput.x < -0.01f)
        {
            FaceLeft();
        }
    }
    private void Update()
    {
        bool canMove =
        (playerFocus == null || !playerFocus.IsFocusing) &&
        (playerAttack == null || !playerAttack.IsAttacking);
        bool isMoving = canMove && moveInput.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("1_Move", isMoving);
        }
    }

    private void FixedUpdate()
    {
        bool cannotMove =
            (playerFocus != null && playerFocus.IsFocusing) ||
            (playerAttack != null && playerAttack.IsAttacking);

        if (cannotMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
    private void FaceRight()
    {
        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x = defaultFaceLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}