using UnityEngine;
using MagicVillageDash.World;

namespace MagicVillageDash.Collectibles
{
    /// <summary>
    /// Per-chunk helper that asks the CoinRailGenerator to spawn the portion of
    /// the coin rail that lies within this chunk's Z range.
    /// </summary>
    [RequireComponent(typeof(ChunkRoot))]
    public sealed class CoinRailFiller : MonoBehaviour
    {
        [SerializeField] private CoinRailGenerator generator;
        [SerializeField] private ChunkRoot chunk;
        private bool spawned;

        void Reset()
        {
            chunk = GetComponent<ChunkRoot>();
        }

        void OnEnable()
        {
            
            if (!chunk) chunk = GetComponent<ChunkRoot>();
            if (!generator) generator = FindAnyObjectByType<CoinRailGenerator>();

            if (chunk && generator)
            {
                float startZ = transform.position.z;
                float endZ = startZ + Mathf.Max(0.01f, chunk.ChunkLength);
                generator.FillRange(transform, startZ, endZ);
                spawned = true;
            }
        }

        // Called by ChunkRoot.ResetForPool() if you want a hard reset, but OnEnable() already re-spawns per activation.
        public void ClearSpawnedFlag() => spawned = false;
    }
}
