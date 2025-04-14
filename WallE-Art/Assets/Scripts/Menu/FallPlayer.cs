using UnityEngine;

public class FallPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 posicionJugador = other.transform.position;
            posicionJugador.y *= -1;
            other.transform.position = posicionJugador; 
        }
    }
}
