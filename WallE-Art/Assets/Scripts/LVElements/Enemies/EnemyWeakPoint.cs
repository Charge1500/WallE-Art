using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    private IEnemyPlatformer parentEnemy;

    [SerializeField] private string playerTag = "Player";

    void Awake()
    {
        parentEnemy = GetComponentInParent<IEnemyPlatformer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Player player = other.GetComponent<Player>();
            if(!player.isChangingScale && !player.dead){
                parentEnemy.Defeat(player);
            }
        }
    }
}
