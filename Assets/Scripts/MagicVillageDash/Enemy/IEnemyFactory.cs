using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public interface IEnemyFactory
    {
        EnemyController Spawn(int laneIndex);
        EnemyController Spawn(Vector3 position, Quaternion rotation);
        public void Recycle(EnemyController instance);
    }
}