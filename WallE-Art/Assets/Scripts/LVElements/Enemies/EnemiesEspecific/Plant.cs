using UnityEngine;

public class Plant : MonoBehaviour
{
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Damage();
        }
    }

}
