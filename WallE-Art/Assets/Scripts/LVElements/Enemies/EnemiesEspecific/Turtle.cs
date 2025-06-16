using UnityEngine;
using System.Collections;

public class Turtle : EnemyPlatformerBase
{
    public bool isHide = false;
    public bool isHidingMove = false;

    public override void Defeat(Player player)
    {
        if (isHide)
        {
            player.BounceOnEnemy(combat.PlayerBounceForce);
            stateController.SetDefeated(true);
            components.mainCollider.enabled = false;
            StartCoroutine(DestroyAfterDelayCoroutine(combat.DestroyDelay));
        }
        else 
        {
            player.BounceOnEnemy(combat.PlayerBounceForce);
            stateController.SetDefeated(true);
            
            animations.SetDead(true); 
            movement.SetMoveSpeed(0f);
            components.rb.linearVelocity = Vector2.zero;
            
            isHide = true;
        }
    }

    public override void HandlePlayerContact(Player player)
    {

        if (!isHide) player.Damage();
        else if (isHidingMove) player.Damage();
        else if (isHide)
        {
            if ((movement.IsFacingRight && !(player.transform.localScale.x > 0)) || (!movement.IsFacingRight && player.transform.localScale.x > 0))
            {
                movement.Flip();
            }
            float playerDirection = Mathf.Sign(player.transform.localScale.x);

            movement.isFacingRight = player.transform.localScale.x>0;

            movement.SetMoveSpeed(3f);
            stateController.SetDefeated(false); 
            isHidingMove = true; 
        }
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            if (stateController.IsDefeated && !isHide) 
            {
                return;
            }
            HandlePlayerContact(player);
        }
    }
}
