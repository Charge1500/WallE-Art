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
                other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
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
                other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            }
        }
    }
}
