using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public interface IEnemyFactory
    {
        EnemyController Spawn(int laneIndex);
        public void Recycle(EnemyController instance);
    }
}