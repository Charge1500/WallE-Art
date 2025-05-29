using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IEnemyPlatformer enemy = other.GetComponent<IEnemyPlatformer>();
            if (!enemy.IsDefeated)
            {
                enemy.SetPlayerProximity(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IEnemyPlatformer enemy = other.GetComponent<IEnemyPlatformer>();
            if (!enemy.IsDefeated)
            {
                enemy.SetPlayerProximity(false);
            }
        }
    }
}
