using UnityEngine;

public interface IEnemyPlatformer
{
    bool IsDefeated { get; }

    void Defeat(Player player);

    void HandlePlayerContact(Player player);
}
