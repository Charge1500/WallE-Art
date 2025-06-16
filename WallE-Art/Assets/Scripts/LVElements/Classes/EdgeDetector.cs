using UnityEngine;

[System.Serializable]
public class EdgeDetector
{
    [Header("Detección de Bordes")]
    [SerializeField] private Transform frontEdgeDetector;
    [SerializeField] private Transform frontWallDetector;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] public float wallCheckDistance = 0.1f;
    
    public void Initialize(Transform edgeDetectorTransform, Transform wallDetectorTransform, LayerMask layer, float wallDistance)
    {
        frontEdgeDetector = edgeDetectorTransform;
        frontWallDetector = wallDetectorTransform;
        obstacleLayer = layer;
        wallCheckDistance = wallDistance;
    }

    public bool IsGroundAhead()
    {
        if (frontEdgeDetector == null) return true;
        return Physics2D.OverlapCircle(frontEdgeDetector.position, 0.1f, obstacleLayer);
    }
    
    public bool IsWallAhead(bool facingRight)
    {
        if (frontWallDetector == null) return false;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(frontWallDetector.position, direction, wallCheckDistance, obstacleLayer);
        return hit.collider != null && !hit.collider.isTrigger;
    }
}