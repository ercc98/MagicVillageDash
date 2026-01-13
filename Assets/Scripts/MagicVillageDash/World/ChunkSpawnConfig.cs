using UnityEngine;

namespace MagicVillageDash.World
{
    [CreateAssetMenu(menuName = "MagicVillageDash/Data/Chunk Spawn Config")]
    public sealed class ChunkSpawnConfig : ScriptableObject
    {
        [Header("Spawn Distances")]
        public int keepAhead = 6;
        public float despawnBehindDistance = 40f;
        public float startAheadDistance = 10f;
    }
}
