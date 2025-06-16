using UnityEngine;

[System.Serializable]
public class EntityComponents
{
    [Header("Componentes Básicos")]
    public Rigidbody2D rb;
    public Animator animator;
    public Collider2D mainCollider;
    
    public void InitializeComponents(GameObject gameObject)
    {
        if (rb == null) rb = gameObject.GetComponent<Rigidbody2D>();
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        if (mainCollider == null) mainCollider = gameObject.GetComponent<Collider2D>();
    }
}