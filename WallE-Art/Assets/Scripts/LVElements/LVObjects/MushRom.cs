using UnityEngine;

public class MushRom : EnemyPlatformerBase
{
    [SerializeField] private (float x,float y) scalePlayer = (1.25f,1.25f);
    [SerializeField] private float timeAlive = 10f;
    void Start()
    {
        stateController.SetPlayerProximity(true); 
        StartCoroutine(DestroyAfterDelayCoroutine(timeAlive)); 
    }

    public override void HandlePlayerContact(Player player)
    {
        player.ChangeScale(scalePlayer.x, scalePlayer.y);
        DestroySelf();
    }
}
