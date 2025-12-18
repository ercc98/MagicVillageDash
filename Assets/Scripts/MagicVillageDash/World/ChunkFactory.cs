using UnityEngine;
using ErccDev.Foundation.Core.Factories;

namespace MagicVillageDash.World
{
    public sealed class ChunkFactory : Factory<ChunkRoot>
    {
        [SerializeField] private GameObject worldMover;
        public override ChunkRoot Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var chunk = base.Spawn(position, rotation, worldMover.transform);
            return chunk;
        }

        public override void Recycle(ChunkRoot instance)
        {
            if (instance) instance.ResetForPool();
            base.Recycle(instance);
        }
    }
}
