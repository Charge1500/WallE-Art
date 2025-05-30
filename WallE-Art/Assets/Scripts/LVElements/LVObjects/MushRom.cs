using UnityEngine;

public class MushRom : EnemyPlatformerBase
{
    void Start(){
        isPlayerNear = true;
        StartCoroutine(DestroyAfterDelayCoroutine(10f));
    }
    public override void HandlePlayerContact(Player player)
    {
        player.ChangeScale(1.2f,1.2f);
        Destroy(gameObject);
    }
}
