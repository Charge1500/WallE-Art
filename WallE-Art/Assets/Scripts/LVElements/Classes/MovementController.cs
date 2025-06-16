using UnityEngine;

[System.Serializable]
public class MovementController
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public bool isFacingRight = true;
    
    private Transform transform;
    private Rigidbody2D rb;
    
    public float MoveSpeed => moveSpeed;
    public bool IsFacingRight => isFacingRight;
    
    public void Initialize(Transform entityTransform, Rigidbody2D rigidbody)
    {
        transform = entityTransform;
        rb = rigidbody;
    }
    
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    
    public void Move(float direction)
    {
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }
    
    public void Stop()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
    
    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    public bool ShouldFlip(float inputDirection)
    {
        return (inputDirection > 0 && !isFacingRight) || (inputDirection < 0 && isFacingRight);
    }
}