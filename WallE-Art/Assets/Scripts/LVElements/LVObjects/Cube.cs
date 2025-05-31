using UnityEngine;
using System.Collections; 

public class Cube : MonoBehaviour
{
    [Header("Configuración de Items")]
    public GameObject[] itemsToDrop; 
    public Transform pointItemSpawn;
    public LayerMask ground;

    [Header("Configuración de Animación y Estado")]
    public Animator animator; 
    public string nameTriggerDesactive = "Off";

    public float jumpHeight = 0.5f; 
    public float jumpSpeed = 2f; 

    private Vector3 originalPosition;
    public bool canHit = true;

    void Start()
    {
        originalPosition = transform.position;
        animator = GetComponent<Animator>();
        Vector2 raycastOrigin = transform.position;
        Vector2 raycastDirection = Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(pointItemSpawn.position, raycastDirection, 0.5f, ground);
        if(hit.collider != null){
            canHit=false;
            animator.SetTrigger(nameTriggerDesactive);
        }
    }
    /* void Update(){
        Vector2 raycastOrigin = transform.position;
        Vector2 raycastDirection = Vector2.up;
        Debug.DrawRay(raycastOrigin, raycastDirection * 0.5f, Color.red);
    } */
    public void HitCube()
    {
        if (!canHit) return;
        
        canHit = false;
        StartCoroutine(JumpAnimation());
        DropItem();
        animator.SetTrigger(nameTriggerDesactive);
    }

    public IEnumerator JumpAnimation()
    {
        Vector3 objectivePosition = originalPosition + Vector3.up * jumpHeight;

        while (transform.position != objectivePosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, objectivePosition, jumpSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position != originalPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, jumpSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void DropItem()
    {
        int index = Random.Range(0, itemsToDrop.Length); 
        Instantiate(itemsToDrop[index], pointItemSpawn.position, Quaternion.identity);
    }
}