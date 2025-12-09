using UnityEngine;
using ErccDev.Foundation.Core.Factories;

namespace MagicVillageDash.World
{
    public sealed class ChunkFactory : Factory<ChunkRoot>
    {
        public override ChunkRoot Spawn(Vector3 position, Quaternion rotation)
        {
            var chunk = base.Spawn(position, rotation);
            return chunk;
        }

        public override void Recycle(ChunkRoot instance)
        {
            if (instance) instance.ResetForPool();
            base.Recycle(instance);
        }
    }
}
