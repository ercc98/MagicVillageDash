using UnityEngine;
using ErccDev.Foundation.Core.Factories;
using MagicVillageDash.World;          // your generic Factory<T>

namespace MagicVillageDash.Collectibles
{
    /// <summary>
    /// Spawns pooled coins. When a coin is collected, the factory automatically recycles it.
    /// Coins are parented under the provided chunk so they move with the world.
    /// </summary>
    public sealed class CoinFactory : Factory<CoinCollectible>
    {
    
        protected override void Awake()
        {
            base.Awake();          // ensures WarmPool()
            
        }

        public CoinCollectible Spawn(Transform parent, Vector3 position, Quaternion rotation, bool worldSpace = true)
        {
            var coin = base.Spawn(position, rotation);
            coin.transform.SetParent(parent, worldSpace);
            
            var chunk = parent.GetComponentInParent<ChunkRoot>();
            coin.Owner = chunk;
            if (chunk) chunk.Register(coin);

            coin.Collected += HandleCollected;
            return coin;
        }

        public CoinCollectible Spawn(Transform parent)
            => Spawn(parent, parent.position, parent.rotation, true);

        public override void Recycle(CoinCollectible instance)
        {
            if (!instance) return;
            instance.Collected -= HandleCollected;

            var chunk = instance.Owner;
            if (chunk) chunk.Unregister(instance);
            instance.Owner = null;

            instance.transform.SetParent(transform, false);
            base.Recycle(instance);
        }

        void HandleCollected(CoinCollectible coin, GameObject _) => Recycle(coin);
    }
}