using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public Cube cube;

    void Start()
    {       
        cube = GetComponentInParent<Cube>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            cube.HitCube();
        }
    }
}