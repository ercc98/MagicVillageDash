using System;
using MagicVillageDash.Enemy;

public interface IEnemySpawner
{
    void Spawn();
    event Action<EnemyController> OnSpawned;
    event Action<int> OnStartSpawn;
}