using UnityEngine;

public static class EntityUtils
{
    public static void FreezeEntity(Rigidbody2D rb, Collider2D collider)
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        collider.enabled = false;
    }
    
    public static void UnfreezeEntity(Rigidbody2D rb, Collider2D collider)
    {
        rb.gravityScale = 1f;
        collider.enabled = true;
    }
    
    public static Collider2D[] DetectEntitiesInRadius(Vector2 center, float radius, LayerMask layer)
    {
        return Physics2D.OverlapCircleAll(center, radius, layer);
    }
    
    public static bool CheckObstacleAbove(Vector2 origin, float distance, LayerMask obstacleLayer)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, distance, obstacleLayer);
        return hit.collider == null;
    }
}