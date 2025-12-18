using System.Collections.Generic;
using MagicVillageDash.Collectibles;
using MagicVillageDash.Obstacles;
using UnityEngine;

namespace MagicVillageDash.World
{
    public sealed class ChunkRoot : MonoBehaviour
    {
        [SerializeField] private ChunkContentSpawner contentSpawner;
        
        // Registries (no Transform scans)
        internal readonly List<CoinCollectible> coins = new();
        internal readonly List<ObstacleHazard>  obstacles = new();
        
        // Factories are injected once by the spawner
        private CoinFactory coinFactory;
        private ObstacleFactory obstacleFactory;
        public float ChunkLength { get; set; } = 24f;
        public ChunkFactory OwnerFactory { get; internal set; } 

        void OnEnable()
        {
            contentSpawner.ChunkLength = ChunkLength;
            contentSpawner?.Spawn();
            if(coinFactory == null) coinFactory = FindAnyObjectByType<CoinFactory>();
            if(obstacleFactory == null) obstacleFactory = FindAnyObjectByType<ObstacleFactory>();
        }

        /// <summary>Called by ChunkSpawner right after spawning the chunk.</summary>
        public void InjectFactories(CoinFactory coinF, ObstacleFactory obsF)
        {
            coinFactory = coinF;
            obstacleFactory = obsF;
        }

        // --- Registry API (called by factories on spawn/recycle) ---
        public void Register(CoinCollectible c)   { if (c && !coins.Contains(c)) coins.Add(c); }
        public void Unregister(CoinCollectible c) { if (c) coins.Remove(c); }
        public void Register(ObstacleHazard h)    { if (h && !obstacles.Contains(h)) obstacles.Add(h); }
        public void Unregister(ObstacleHazard h)  { if (h) obstacles.Remove(h); }


        public void ResetForPool()
        {
            // Recycle any leftover residents without scanning transforms
            ResetCoinsForPool();
            ResetObstaclesForPool();
        }

        public void ResetCoinsForPool()
        {            
            if (coinFactory != null)
            {
                for (int i = coins.Count - 1; i >= 0; i--)
                    if (coins[i]) coinFactory.Recycle(coins[i]);
            }
            coins.Clear();
        }

        public void ResetObstaclesForPool()
        {
            if (obstacleFactory != null)
            {
                for (int i = obstacles.Count - 1; i >= 0; i--)
                    if (obstacles[i]) obstacleFactory.Recycle(obstacles[i]);
            }
            obstacles.Clear();
        }
    }
}
