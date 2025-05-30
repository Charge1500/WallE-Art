using UnityEngine;

public interface IEnemyPlatformer
{
    bool IsDefeated { get; }
    
    void SetPlayerProximity(bool isNear);

    void Defeat(Player player);

    void HandlePlayerContact(Player player);
}
