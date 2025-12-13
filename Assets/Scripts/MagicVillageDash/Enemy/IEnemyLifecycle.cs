using System;
using MagicVillageDash.Enemy;
using UnityEngine;

namespace MagicVillageDash
{
    public interface IEnemyLifecycle
    {
        event Action<EnemyController> OnDied;

        // Call when enemy should be removed (hp <= 0, hit, timeout, etc.)
        void Kill();

        // Optional hooks for pooling lifecycle
        void OnSpawned();
        void OnRecycled();
    }
}
