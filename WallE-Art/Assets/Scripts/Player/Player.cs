using UnityEngine;
using System.Collections;

public class PlayerController2D : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMove = 0f;
    private bool isFacingRight = true;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded = false;

    [Header("Agacharse")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl; 
    private bool isCrouching = false;

    [Header("Inactividad")]
    [SerializeField] private float inactivityThreshold = 5f; 
    private float timeSinceLastAction = 0f;
    private bool isInactive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        HandleInput();
        HandleInactivity();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        if (!isCrouching)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;
        }
        else
        {
            horizontalMove = 0f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            ResetInactivityTimer(); 
        }

        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            isCrouching = true;
            ResetInactivityTimer(); 
        }
        else if (Input.GetKeyUp(crouchKey) || !isGrounded)
        {
            if (isCrouching)
            {
                 isCrouching = false;
            }
        }
        if (Mathf.Abs(horizontalMove) > 0.01f)
        {
            ResetInactivityTimer();
        }
    }

    void HandleMovement()
    {
        rb.linearVelocity = new Vector2(horizontalMove, rb.linearVelocity.y);
        if (horizontalMove > 0 && !isFacingRight && !isCrouching)
        {
            Flip();
        }
        else if (horizontalMove < 0 && isFacingRight && !isCrouching)
        {
            Flip();
        }
    }

    void HandleInactivity()
    {
        timeSinceLastAction += Time.deltaTime;
        if (timeSinceLastAction >= inactivityThreshold)
        {
            isInactive = true;
        }
        else
        {
            isInactive = false;
        }
    }

    void ResetInactivityTimer()
    {
        timeSinceLastAction = 0f;
        isInactive = false;
        animator.SetBool("IsInactive", false);
    }

    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }

        animator.SetBool("IsCrouching", isCrouching);

        if (isInactive && isGrounded && !isCrouching && Mathf.Abs(horizontalMove) < 0.01f && !animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsInactive", true);
        }
        else
        {
             if (animator.GetBool("IsInactive") && !isInactive)
             {
                animator.SetBool("IsInactive", false);
             }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
