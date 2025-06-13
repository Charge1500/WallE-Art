using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public Animator animator;
    [SerializeField] private LevelManager lvManager;
    [SerializeField] private LayerMask enemiesLayer;

    [Header("Movimiento")]
    [SerializeField] protected Collider2D mainCollider;
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMove = 0f;
    private bool isFacingRight = true;
    public bool dead = false;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private bool isGrounded = false;

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

    /* void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 8f);
    } */

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
        if(isGrounded && rb.linearVelocity.y <= 0f) animator.SetBool("IsJumping", false);
        if (!isGrounded && rb.linearVelocity.y <= 0)
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
        if(Mathf.Abs(transform.localScale.x)!=1){
            int face=1;
            if(!isFacingRight) face=-1;
            transform.localScale = new Vector2(1f*face,1f);
            return;
        }
        Dead();
        
    }
    public void Dead(){
        dead=true;
        DetectEnemies(false);
        StartCoroutine(DeadAnim());
        StartCoroutine(ActiveScreen(0,2.5f));
        StartCoroutine(DesactivePlayer(2.5f));
    }
    public IEnumerator DeadAnim(){
        rb.gravityScale = 0f;
        WalleStop();
        animator.SetBool("IsCrouching", true);
        yield return new WaitForSeconds(1f);
        rb.gravityScale = 1f;
        BounceOnEnemy(3.5f);
        mainCollider.enabled = false;
    }

    public IEnumerator ActiveScreen(int screen,float time){
        yield return new WaitForSeconds(time);   
        lvManager.screens[screen].SetActive(true);
        lvManager.cinemachineCamera.Follow = null; 
    }

    public IEnumerator DesactivePlayer(float time){
        yield return new WaitForSeconds(time);   
        gameObject.SetActive(false); 
    }

    public void BounceOnEnemy(float bounceAmount){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceAmount);
        animator.SetBool("IsJumping", true);
    }

    public void WalleStop(){
        rb.linearVelocity = new Vector2(0f, 0f);
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsInactive", false);
        animator.SetBool("IsCrouching", false);
    }
    
    public void WalleWin(){
        DetectEnemies(false);
        dead = true;
        WalleStop();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetBool("IsInactive", true);
        StartCoroutine(ActiveScreen(1,4.5f));
    }
    public void ChangeScale(float x, float y){
        if(CheckGrowUp(0.5f,groundLayer)){
            int face=1;
            if(!isFacingRight) face=-1;
            transform.localScale = new Vector2(x*face,y);
        }
    }  

    public bool CheckGrowUp(float raycastDistance,LayerMask whatIsObstacle)
    {
        Vector2 raycastOrigin = transform.position;
        Vector2 raycastDirection = Vector2.up;
        //Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.red);
        
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastDistance, whatIsObstacle);
        if (hit.collider != null)
        {
            return false;
        }
        return true;
    }

    public void DetectEnemies(bool active){
        Collider2D[] objects = Physics2D.OverlapCircleAll(groundCheck.position, 10f, enemiesLayer);
        foreach (Collider2D other in objects)
        { 
            if(other.CompareTag("Enemy")){
                IEnemyPlatformer enemy = other.GetComponent<IEnemyPlatformer>();
                if (!enemy.IsDefeated)
                {
                    enemy.SetPlayerProximity(active);
                    other.gameObject.GetComponent<Rigidbody2D>().constraints = (active) ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
    }
}
