using UnityEngine;

[System.Serializable]
public class AnimationController
{
    private Animator animator;
    
    [Header("Parámetros de Animator")]
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string jumpParam = "IsJumping";
    [SerializeField] private string fallParam = "IsFalling";
    [SerializeField] private string crouchParam = "IsCrouching";
    [SerializeField] private string deadParam = "Dead";
    [SerializeField] private string inactiveParam = "IsInactive";
    
    public void Initialize(Animator anim)
    {
        animator = anim;
    }
    
    public void SetSpeed(float speed) => animator?.SetFloat(speedParam, speed);
    public void SetJumping(bool isJumping) => animator?.SetBool(jumpParam, isJumping);
    public void SetFalling(bool isFalling) => animator?.SetBool(fallParam, isFalling);
    public void SetCrouching(bool isCrouching) => animator?.SetBool(crouchParam, isCrouching);
    public void SetDead(bool isDead) => animator?.SetBool(deadParam, isDead);
    public void SetInactive(bool isInactive) => animator?.SetBool(inactiveParam, isInactive);
    
    public bool GetBool(string param) => animator?.GetBool(param) ?? false;
}