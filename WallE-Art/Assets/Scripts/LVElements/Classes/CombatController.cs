using UnityEngine;

[System.Serializable]
public class CombatController
{
    [Header("Configuración de Combate")]
    [SerializeField] private float playerBounceForce = 5f; // Renombrado para claridad
    [SerializeField] private float destroyDelay = 1f;
    
    public float PlayerBounceForce => playerBounceForce;
    public float DestroyDelay => destroyDelay;
    
    public void ApplyBounce(Rigidbody2D playerRb)
    {
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, playerBounceForce);
    }
}