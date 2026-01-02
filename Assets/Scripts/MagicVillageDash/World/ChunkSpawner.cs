using System.Collections.Generic;
using MagicVillageDash.Collectibles;
using MagicVillageDash.Obstacles;
using UnityEngine;

namespace MagicVillageDash.World
{
    public sealed class ChunkSpawner : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] Transform player;        // stationary
        [SerializeField] CoinRailFiller coinRailFiller;
        [SerializeField] ObstacleRailFiller obstacleRailFiller;
        [SerializeField] private ChunkFactory[] factories;  // multiple factories (no weights)

        //[Header("Layout")]
        float chunkLength = 24f;

        [Header("Runway")]
        [SerializeField] int keepAhead = 6;
        [SerializeField] float despawnBehindDistance = 40f;
        [SerializeField] float startAheadDistance = 10f;
        readonly List<ChunkRoot> active = new();
        int nextSpawnZ;

        IChunkFiller coinFiller;
        IChunkFiller obstacleFiller;

        void OnEnable()
        {
            coinFiller = coinRailFiller;
            obstacleFiller = obstacleRailFiller;
        }

        void Start()
        {
            nextSpawnZ = (int)(player.position.z + startAheadDistance);
            FillOneAhead(false);
            nextSpawnZ += (int)chunkLength;
            FillAhead();
            //nextSpawnZ += (int)chunkLength;
            //FillOneAhead();
            //nextSpawnZ -= (int)chunkLength;
            FindAnyObjectByType<CoinRailGenerator>()?.ResetPathAt(player.position.z);

        }

        void FixedUpdate()
        {
            if (active.Count > 0)
            {
                var first = active[0];
                if (player.position.z - first.transform.position.z > despawnBehindDistance)
                {
                    active.RemoveAt(0);
                    first.OwnerFactory?.Recycle(first);  // recycle to the same factory
                    FillOneAhead();
                }
            }
        }

        void FillAhead()
        {
            while (active.Count < keepAhead)
            {
                FillOneAhead();
                nextSpawnZ += (int)chunkLength;
            }
            nextSpawnZ -= 1 * (int)chunkLength;
        }

        ChunkRoot FillOneAhead(bool spawnObstacles = true)
        {
            var factory = GetRandomFactory();
            ChunkRoot chunk = factory.Spawn(new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);

            coinFiller.FillChunk(chunk);
            if(spawnObstacles) obstacleFiller.FillChunk(chunk);
            // Mark the owner factory so we can recycle correctly
            chunk.OwnerFactory = factory;
            //chunk.ChunkLength = chunkLength;
            chunkLength = chunk.ChunkLength;
            active.Add(chunk);
            return chunk;
        }
        
        ChunkFactory GetRandomFactory()
        {
            if (factories == null || factories.Length == 0) return null;

            // Pick a random non-null factory
            for (int tries = 0; tries < 8; tries++)
            {
                var f = factories[Random.Range(0, factories.Length)];
                if (f != null) return f;
            }

            // Fallback: first non-null
            foreach (var f in factories)
                if (f != null) return f;

            return null;            
        }
    }
}
