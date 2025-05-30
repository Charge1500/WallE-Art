using UnityEngine;
using System.Collections;

public class Turtle : EnemyPlatformerBase
{
    public bool isHide=false;
    public bool isHidingMove=false;
    public override void Defeat(Player player)
    {
        if(isHide){
            player.BounceOnEnemy(playerBounceOnDefeat);
            _isDefeated = true;
            mainCollider.enabled = false;
            StartCoroutine(DestroyAfterDelayCoroutine(destroyDelayAfterDefeat));
        }
        player.BounceOnEnemy(playerBounceOnDefeat);
        _isDefeated = true;

        animator.SetBool(defeatedAnimatorParam, true);

        moveSpeed = 0f;

        rb.linearVelocity = Vector2.zero;
        isHide = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            HandlePlayerContact(player);
        }
    }

    public override void HandlePlayerContact(Player player)
    {
        if(!isHide) player.Damage();
        if(isHidingMove) player.Damage();
        if(isHide){
            if((isFacingRight && !(player.transform.localScale.x>0)) || (!isFacingRight && player.transform.localScale.x>0)) {
                transform.localScale = new Vector3(transform.localScale.x *-1,transform.localScale.y,0);
            }
            isFacingRight = player.transform.localScale.x>0;
            moveSpeed = 3f; 
            _isDefeated = false;
            isHidingMove = true;
        }
    }

}
