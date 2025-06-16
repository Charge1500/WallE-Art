using UnityEngine;

[System.Serializable]
public class GroundDetector
{
    [Header("Detección de Terreno")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    public bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    
    public void Initialize(Transform groundCheckTransform, LayerMask layer)
    {
        groundCheck = groundCheckTransform;
        groundLayer = layer;
    }
}