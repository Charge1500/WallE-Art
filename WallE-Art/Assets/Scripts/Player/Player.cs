using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LevelManager lvManager;

    [Header("Movimiento")]
    [SerializeField] protected Collider2D mainCollider;
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMove = 0f;
    private bool isFacingRight = true;
    private bool dead = false;

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
        mainCollider = GetComponent<Collider2D>();
        lvManager = GetComponentInParent<LevelManager>();
        groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        if(!dead){
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            HandleInput();
            HandleInactivity();
            UpdateAnimations();
        }
    }

    void FixedUpdate()
    {
        if(!dead) HandleMovement();
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
    public void Damage(){
        Dead();
    }
    public void Dead(){
        dead=true;
        StartCoroutine(DeadAnim());
        StartCoroutine(GameOverScreen());
        
    }
    public IEnumerator DeadAnim(){
        DetectEnemies(false);
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(0f, 0f);
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsInactive", false);
        animator.SetBool("IsCrouching", true);
        yield return new WaitForSeconds(1f);
        rb.gravityScale = 1f;
        BounceOnEnemy(3.5f);
        mainCollider.enabled = false;
    }
    public IEnumerator GameOverScreen(){
        yield return new WaitForSeconds(2.5f);

        lvManager.gameOverScreen.SetActive(true);
        lvManager.cinemachineCamera.Follow = null;    
        
        Time.timeScale = 0;
    }
    public void BounceOnEnemy(float bounceAmount){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceAmount);
        animator.SetBool("IsJumping", true);
    }

    public void DetectEnemies(bool active){
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, 10f);
        foreach (Collider2D other in objects)
        {
            if (other.CompareTag("Enemy"))
            {
                    IEnemyPlatformer enemy = other.GetComponent<IEnemyPlatformer>();
                if (!enemy.IsDefeated)
                {
                    enemy.SetPlayerProximity(active);
                }
            }
        }
    }
    
    
}
