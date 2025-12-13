using ErccDev.Foundation.Core.Factories;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemyFactory : Factory<EnemyController>
    {

        protected override void Awake()
        {
            base.Awake();         
        }

        public EnemyController Spawn(Transform parent, Vector3 position, Quaternion rotation, bool worldSpace = true)
        {
            var spawnedEnemy = base.Spawn(position, rotation);
            spawnedEnemy.transform.SetParent(parent, worldSpace);
            return spawnedEnemy;
        }

        public EnemyController Spawn(int laneIndex)
        {
            EnemyController spawnedEnemy = Spawn(transform, new Vector3(0f, 0f, 0f), Quaternion.identity, true);
            spawnedEnemy.SetSpawnPose(laneIndex);
            spawnedEnemy.Ondied += HandleOndied; 
            return spawnedEnemy;
        }

        public EnemyController Spawn(Transform parent)
            => Spawn(parent, parent.position, parent.rotation, true);

        public override void Recycle(EnemyController instance)
        {
            if (!instance) return;
            instance.transform.SetParent(transform, false);
            instance.Ondied -= HandleOndied;
            base.Recycle(instance);
        }

        void HandleOndied(EnemyController enemy) => Recycle(enemy);
    }
}
