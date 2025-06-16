using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Componentes Modulares")]
    [SerializeField] private EntityComponents components;
    [SerializeField] private MovementController movement;
    [SerializeField] private GroundDetector groundDetector;
    [SerializeField] private AnimationController animations;
    
    [Header("Configuración Específica del Jugador")]
    [SerializeField] private LevelManager lvManager;
    [SerializeField] private LayerMask enemiesLayer;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private KeyCode crouchKey = KeyCode.DownArrow; 
    [SerializeField] private LayerMask groundLayer; 

    [Header("Audio")]
    [SerializeField] public AudioClip[] mySoundsClip; 
    [SerializeField] private AudioSource audioSource; 
    
    // Estados específicos del jugador
    public bool dead = false;
    public bool isChangingScale = false;
    private bool isCrouching = false;
    private float horizontalInput = 0f;

    [Header("Inactividad")]
    [SerializeField] private float inactivityThreshold = 5f; 
    private float timeSinceLastAction = 0f;
    private bool isInactive = false;
    
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        components.InitializeComponents(gameObject);
        movement.Initialize(transform, components.rb);
        groundDetector.Initialize(transform.Find("GroundCheck"), groundLayer);
        animations.Initialize(components.animator);
        lvManager = FindFirstObjectByType<LevelManager>();

    }
    
    void Update()
    {
        if (!dead)
        {
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
        if (!dead) HandleMovement();
    }
    
    private void HandleInput()
    {
        horizontalInput = isCrouching ? 0f : Input.GetAxisRaw("Horizontal");
        
        if (Input.GetButtonDown("Jump") && groundDetector.IsGrounded && !isCrouching)
        {
            audioSource.PlayOneShot(mySoundsClip[0]);
            components.rb.linearVelocity = new Vector2(components.rb.linearVelocity.x, jumpForce);
            animations.SetJumping(true);
            ResetInactivityTimer(); 
        }
        
        if (Input.GetKeyDown(crouchKey) && groundDetector.IsGrounded)
        {
            isCrouching = true;
            ResetInactivityTimer(); 
        }
        else if (Input.GetKeyUp(crouchKey) || !groundDetector.IsGrounded)
        {
            if (isCrouching && EntityUtils.CheckObstacleAbove(transform.position, 0.5f, groundLayer))
            {
                isCrouching = false;
            } else if (isCrouching && !groundDetector.IsGrounded) {
                isCrouching = false;
            }
        }

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            ResetInactivityTimer();
        }
    }
    
    private void HandleMovement()
    {
        movement.Move(horizontalInput);
        
        if (movement.ShouldFlip(horizontalInput) && !isCrouching)
        {
            movement.Flip();
        }
    }

    private void HandleInactivity()
    {
        timeSinceLastAction += Time.deltaTime;
        if (timeSinceLastAction >= inactivityThreshold && !animations.GetBool("IsJumping") && groundDetector.IsGrounded && Mathf.Abs(horizontalInput) < 0.01f && !isCrouching)
        {
            isInactive = true;
            animations.SetInactive(true);
        }
        else
        {
            if (isInactive && (animations.GetBool("IsJumping") || !groundDetector.IsGrounded || Mathf.Abs(horizontalInput) > 0.01f || isCrouching))
            {
                isInactive = false;
                animations.SetInactive(false);
            }
        }
    }

    private void ResetInactivityTimer()
    {
        timeSinceLastAction = 0f;
        isInactive = false;
        animations.SetInactive(false);
    }
    
    private void UpdateAnimations()
    {
        animations.SetSpeed(Mathf.Abs(horizontalInput * movement.MoveSpeed));
        animations.SetCrouching(isCrouching);
        
        if (groundDetector.IsGrounded && components.rb.linearVelocity.y <= 0f)
        {
            animations.SetJumping(false);
            animations.SetFalling(false);
        }
        
        if (!groundDetector.IsGrounded && components.rb.linearVelocity.y < 0)
        {
            animations.SetFalling(true);
            animations.SetJumping(false);
        }
    }

    public void Damage()
    {
        if (transform.localScale.y != 1)
        {
            ChangeScale(1f, 1f);
            return;
        }
        Die();
        MusicManager.Instance.StopMusic();
        audioSource.PlayOneShot(mySoundsClip[1]);
    }
    
    public void Die()
    {
        dead = true;
        DetectEnemies(false);
        StartCoroutine(DeadAnim());
        StartCoroutine(ActiveScreen(0, 2.5f));
        StartCoroutine(DesactivePlayer(2.5f));
    }

    public IEnumerator DeadAnim()
    {
        EntityUtils.FreezeEntity(components.rb, components.mainCollider);
        WalleStop();
        animations.SetCrouching(true); 
        yield return new WaitForSeconds(1f);
        components.rb.gravityScale = 1f;
        BounceOnEnemy(3.5f);
        components.mainCollider.enabled = false;
    }

    public IEnumerator ActiveScreen(int screen, float time)
    {
        yield return new WaitForSeconds(time);   
        if (lvManager != null)
        {
            lvManager.screens[screen].SetActive(true);
            if (lvManager.cinemachineCamera != null)
            {
                lvManager.cinemachineCamera.Follow = null; 
            }
        }
    }

    public IEnumerator DesactivePlayer(float time)
    {
        yield return new WaitForSeconds(time);   
        gameObject.SetActive(false); 
    }

    public void BounceOnEnemy(float bounceAmount)
    {
        components.rb.linearVelocity = new Vector2(components.rb.linearVelocity.x, bounceAmount);
        animations.SetJumping(true);
    }

    public void WalleStop()
    {
        components.rb.linearVelocity = new Vector2(0f, 0f);
        animations.SetSpeed(0);
        animations.SetJumping(false);
        animations.SetFalling(false);
        animations.SetInactive(false);
        animations.SetCrouching(false);
    }
    
    public void WalleWin()
    {
        MusicManager.Instance.StopMusic();
        audioSource.PlayOneShot(mySoundsClip[2]);
        DetectEnemies(false);
        dead = true;
        WalleStop();
        components.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animations.SetInactive(true);
        StartCoroutine(ActiveScreen(1, 4.5f));
    }

    public void ChangeScale(float targetX, float targetY)
    {
        if (!isChangingScale && (EntityUtils.CheckObstacleAbove(transform.position, 0.5f, groundLayer) || transform.localScale.y != 1f))
        {
            if(transform.localScale.y==1) audioSource.PlayOneShot(mySoundsClip[4]);
            else audioSource.PlayOneShot(mySoundsClip[3]);
            StartCoroutine(GrowAnimation(targetX, targetY));
        }
    }  

    private IEnumerator GrowAnimation(float targetX, float targetY)
    {
        isChangingScale = true;
        int enemiesLayerNumber = Mathf.FloorToInt(Mathf.Log(enemiesLayer.value, 2));
        
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemiesLayerNumber, true);

        int face = 0;

        float animationDuration = 1.0f; 
        float blinkDuration = 0.1f; 
        float timer = 0f;

        while (timer < animationDuration)
        {
            face = movement.IsFacingRight ? 1 : -1;
            if (Mathf.RoundToInt(timer / blinkDuration) % 2 == 0)
            {
                transform.localScale = new Vector2(1.2f, 1.2f);
            }
            else
            {
                transform.localScale = new Vector2(0.8f, 0.8f);
            }
            
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * face, transform.localScale.y);

            timer += Time.deltaTime;
            yield return null;
        }

        face = movement.IsFacingRight ? 1 : -1;

        if(!EntityUtils.CheckObstacleAbove(transform.position, 0.5f, groundLayer)){
            transform.localScale = new Vector2(1f * face, 1f);
        }else{
            transform.localScale = new Vector2(targetX * face, targetY);
        }


        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, transform.localScale.y * 0.3f, enemiesLayer); 
        
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                IEnemyPlatformer enemy = enemyCollider.GetComponent<IEnemyPlatformer>();
                if (enemy != null)
                {
                    isChangingScale = false;
                    Damage(); 
                    yield break;
                }
            }
        }
        
        isChangingScale = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemiesLayerNumber, false);
    }

    public void DetectEnemies(bool active)
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, 10f, enemiesLayer); 
        foreach (Collider2D other in objects)
        { 
            if(other.CompareTag("Enemy"))
            {
                IEnemyPlatformer enemy = other.GetComponent<IEnemyPlatformer>();
                if (!enemy.IsDefeated)
                {
                    enemy.SetPlayerProximity(active);
                    Rigidbody2D enemyRb = other.gameObject.GetComponent<Rigidbody2D>();
                    enemyRb.constraints = (active) ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
    }
}