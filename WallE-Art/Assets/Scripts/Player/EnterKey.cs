using UnityEngine;
public class FollowTarget : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0.45f, 0.45f, 0f);
    void LateUpdate()
    {
        transform.position = target.position + offset;    
    }
}
