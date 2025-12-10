using UnityEngine;
using ErccDev.Foundation.Core.Factories;
using MagicVillageDash.World;
using ErccDev.Foundation.Core.Gameplay;

namespace MagicVillageDash.Obstacles
{
    /// <summary>Pooled obstacle spawner. Recycles when obstacle signals Hit.</summary>
    public sealed class ObstacleFactory : Factory<ObstacleHazard>
    {

        protected override void Awake()
        {
            base.Awake();          // ensures WarmPool()
        }

        public ObstacleHazard Spawn(Transform parent, Vector3 position, Quaternion rotation, bool worldSpace = true)
        {
            var obs = base.Spawn(position, rotation);
            obs.transform.SetParent(parent, worldSpace);
            
            var chunk = parent.GetComponentInParent<ChunkRoot>();
            obs.Owner = chunk;
            if (chunk) chunk.Register(obs);

            obs.Hit += HandleHit;
            return obs;
        }

        public ObstacleHazard Spawn(Transform parent)
            => Spawn(parent, parent.position, parent.rotation, true);

        public override void Recycle(ObstacleHazard instance)
        {
            if (!instance) return;
            instance.Hit -= HandleHit;

            var chunk = instance.Owner;
            if (chunk) chunk.Unregister(instance);
            instance.Owner = null;

            instance.transform.SetParent(transform, false);
            base.Recycle(instance);
        }

        void HandleHit(ObstacleHazard obs, GameObject _)
        {
            Recycle(obs);
            GameEvents.RaiseGameOver(); // <— notify UI
        }
    }
}