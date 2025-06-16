public abstract class StateController
{
    protected bool isActive = true;
    protected bool isDefeated = false;
    
    public bool IsActive => isActive;
    public bool IsDefeated => isDefeated;
    
    public virtual void SetActive(bool active) => isActive = active;
    public virtual void SetDefeated(bool defeated) => isDefeated = defeated;
}