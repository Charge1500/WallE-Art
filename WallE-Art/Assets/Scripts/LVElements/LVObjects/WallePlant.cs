using UnityEngine;

public class WallePlant : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (!player.dead)
            {
                player.WalleWin();
            }
        }
    }
}
