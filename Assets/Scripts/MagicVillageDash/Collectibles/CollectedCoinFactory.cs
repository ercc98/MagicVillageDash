using UnityEngine;
using ErccDev.Core.Factories;          // your generic Factory<T>

namespace MagicVillageDash.Collectibles
{
    /// <summary>
    /// Spawns pooled coins. When a coin is collected, the factory automatically recycles it.
    /// Coins are parented under the provided chunk so they move with the world.
    /// </summary>
    public sealed class CollectedCoinFactory : Factory<CollectedCoinCollectible>
    {
        Transform _poolRoot;

        protected override void Awake()
        {
            base.Awake();          // ensures WarmPool()
            _poolRoot = transform; // where inactive instances live
        }

        public CollectedCoinCollectible Spawn(Transform parent, Vector3 position, Quaternion rotation, bool worldSpace = true)
        {
            var coin = base.Spawn(position, rotation);
            coin.transform.SetParent(parent, worldSpace);
            return coin;
        }

        public CollectedCoinCollectible Spawn(Transform parent)
            => Spawn(parent, parent.position, parent.rotation, true);

        public override void Recycle(CollectedCoinCollectible instance)
        {
            if (!instance) return;
            instance.transform.SetParent(_poolRoot, false);
            base.Recycle(instance);
        }
    }
}