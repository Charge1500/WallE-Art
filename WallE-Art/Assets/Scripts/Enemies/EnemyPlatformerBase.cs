using UnityEngine;
using System.Collections;

public abstract class EnemyPlatformerBase : MonoBehaviour, IEnemyPlatformer
{
    [Header("Components")]
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D mainCollider;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 1.5f;
    [Tooltip("Define qué es considerado 'suelo' para que el enemigo no se caiga de las plataformas.")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform frontEdgeDetector;
    [SerializeField] protected Transform frontWallDetector;
    [SerializeField] protected float wallCheckDistance = 0.1f;


    protected bool isFacingRight = true;
    protected bool isPlayerNear = true;
    protected bool _isDefeated = false;
    public bool IsDefeated => _isDefeated;

    [SerializeField] protected string defeatedAnimatorParam = "Dead";
    [SerializeField] protected float destroyDelayAfterDefeat = 1f;
    [SerializeField] protected float playerBounceOnDefeat = 5f;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCollider = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if(isPlayerNear){
            if (_isDefeated)
            {
                rb.linearVelocity = Vector2.zero; 
                return;    
            }
            PerformMovement();
            CheckForFlipConditions();
        }
    }

    protected virtual void PerformMovement()
    {
        float moveDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    protected virtual void CheckForFlipConditions()
    {
        bool isGroundAhead = Physics2D.OverlapCircle(frontEdgeDetector.position, 0.1f, groundLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(frontWallDetector.position, isFacingRight ? Vector2.right : Vector2.left, wallCheckDistance, groundLayer);


        if (!isGroundAhead || (wallHit.collider != null && !wallHit.collider.isTrigger))
        {
            Flip();
        }
    }
    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public virtual void Defeat(Player player)
    {
        if (_isDefeated) return;

        _isDefeated = true;

        animator.SetBool(defeatedAnimatorParam, true);

        moveSpeed = 0f;

        rb.linearVelocity = Vector2.zero;

        mainCollider.enabled = false;
        rb.gravityScale = 0f;
        
        player.BounceOnEnemy(playerBounceOnDefeat);

        StartCoroutine(DestroyAfterDelayCoroutine(destroyDelayAfterDefeat));
    }

    public virtual void HandlePlayerContact(Player player)
    {
        if (_isDefeated) return;

        player.Damage();
    }

    protected virtual IEnumerator DestroyAfterDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroySelf();
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDefeated) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            HandlePlayerContact(player);
        }
    }
}
