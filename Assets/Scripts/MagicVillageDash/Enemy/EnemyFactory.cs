using ErccDev.Foundation.Core.Factories;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemyFactory : Factory<EnemyController>
    {
        Transform _poolRoot;

        protected override void Awake()
        {
            base.Awake();          // ensures WarmPool()
            _poolRoot = transform; // where inactive instances live
        }

        public EnemyController Spawn(Transform parent, Vector3 position, Quaternion rotation, bool worldSpace = true)
        {
            var spawnedEnemy = base.Spawn(position, rotation);
            spawnedEnemy.transform.SetParent(parent, worldSpace);
            return spawnedEnemy;
        }

        public EnemyController Spawn(Transform parent)
            => Spawn(parent, parent.position, parent.rotation, true);

        public override void Recycle(EnemyController instance)
        {
            if (!instance) return;
            instance.transform.SetParent(_poolRoot, false);
            base.Recycle(instance);
        }
    }
}
