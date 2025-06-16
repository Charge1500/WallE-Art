public class EnemyStateController : StateController
{
    private bool isPlayerNear = false;
    
    public bool IsPlayerNear => isPlayerNear;
    
    public void SetPlayerProximity(bool isNear)
    {
        if (isDefeated) return;
        isPlayerNear = isNear;
    }
}