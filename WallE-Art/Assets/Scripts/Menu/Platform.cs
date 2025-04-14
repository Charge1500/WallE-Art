using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    private Vector3 targetPositionOffset = new Vector3(0f, 5.25f, 0f); 
    [SerializeField] private float moveSpeed = 2.0f;



    private Vector3 startPosition;        
    private Vector3 targetUpPosition;     
    private Vector3 currentTargetPosition;

    private bool isPlayerTouching = false; 

    void Awake()
    {
        startPosition = transform.position;   
        targetUpPosition = startPosition + targetPositionOffset;
        currentTargetPosition = startPosition;
    }

    void Update()
    {
        if (isPlayerTouching)
        {
            currentTargetPosition = targetUpPosition; 
        }
        else
        {
            currentTargetPosition = startPosition;  
        }

        if (Vector3.Distance(transform.position, currentTargetPosition) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTouching = true;

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTouching = false;
        }
    }
}
