using UnityEngine;
using System.Collections;

public abstract class EnemyPlatformerBase : MonoBehaviour, IEnemyPlatformer
{
    [Header("Componentes Modulares")]
    [SerializeField] protected EntityComponents components;
    [SerializeField] protected MovementController movement;
    [SerializeField] protected EdgeDetector edgeDetector;
    [SerializeField] protected EnemyStateController stateController;
    [SerializeField] protected AnimationController animations;
    [SerializeField] protected CombatController combat;

    [Header("Configuración de Capas")]
    [SerializeField] protected LayerMask groundLayer;

    [Header("Audio")]
    [SerializeField] public AudioClip[] mySoundsClip; 
    [SerializeField] private AudioSource audioSource; 
    
    public bool IsDefeated => stateController.IsDefeated;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        components.InitializeComponents(gameObject);
        movement.Initialize(transform, components.rb);
        edgeDetector.Initialize(transform.Find("FrontEdgeDetector"), transform.Find("FrontWallDetector"), groundLayer, edgeDetector.wallCheckDistance); // Asume que tienes GameObjects hijos con estos nombres
        stateController = new EnemyStateController();
        animations.Initialize(components.animator);
    }

    protected virtual void FixedUpdate()
    {
        if (stateController.IsPlayerNear)
        {
            if (stateController.IsDefeated)
            {
                movement.Stop();
                return;    
            }
            PerformMovement();
            CheckForFlipConditions();
        }
        else
        {
            if (!stateController.IsDefeated)
            {
                movement.Stop();
            }
        }
    }

    protected virtual void PerformMovement()
    {
        float moveDirection = movement.IsFacingRight ? 1f : -1f;
        movement.Move(moveDirection);
    }

    protected virtual void CheckForFlipConditions()
    {
        if (!edgeDetector.IsGroundAhead() || edgeDetector.IsWallAhead(movement.IsFacingRight))
        {
            movement.Flip();
        }
    }
    
    public virtual void Defeat(Player player)
    {
        if (stateController.IsDefeated) return;

        stateController.SetDefeated(true);
        animations.SetDead(true);
        movement.SetMoveSpeed(0f);
        EntityUtils.FreezeEntity(components.rb, components.mainCollider);
        
        player.BounceOnEnemy(combat.PlayerBounceForce); 

        StartCoroutine(DestroyAfterDelayCoroutine(combat.DestroyDelay)); 
    }

    public void SetPlayerProximity(bool isNear)
    {
        stateController.SetPlayerProximity(isNear);
        if (!isNear && !stateController.IsDefeated)
        {
            movement.Stop();
        }
    }

    public virtual void HandlePlayerContact(Player player)
    {
        if (stateController.IsDefeated) return;
        player.Damage();
    }

    protected virtual IEnumerator DestroyAfterDelayCoroutine(float delay)
    {
        if(audioSource!=null) audioSource.PlayOneShot(mySoundsClip[0]);
        yield return new WaitForSeconds(delay);
        DestroySelf();
    }

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (stateController.IsDefeated) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            HandlePlayerContact(player);
        }
    }
}