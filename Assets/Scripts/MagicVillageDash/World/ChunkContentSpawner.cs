using MagicVillageDash.Collectibles;
using MagicVillageDash.Obstacles;
using UnityEngine;

namespace MagicVillageDash.World
{
    public sealed class ChunkContentSpawner : MonoBehaviour
    {
        [Header("Lane spawn points inside this chunk (center of lanes/rows)")]
        [SerializeField] Transform[] lanePoints;

        [Header("Factories")]
        [Tooltip("One or more obstacle factories; a random one will be used.")]
        [SerializeField] private ObstacleFactory[] obstacleFactories;
        [SerializeField] private CoinFactory coinFactory;

        [Header("Rules")]
        [Range(0,1)] [SerializeField] float obstacleChancePerLane = 0.45f;
        [Range(0,1)] [SerializeField] private float coinChancePerLane = 0.30f;
        [Range(1,5)] [SerializeField] private int coinLinePerChunk = 1;
        [SerializeField] bool ensureAtLeastOneSafeLane = true;
        public float ChunkLength { get; set; } = 24f;


        public void Spawn()
        {
            if (lanePoints == null || lanePoints.Length == 0) return;

            bool[] blocked = new bool[lanePoints.Length];

            // obstacles
            SpawnObstaclesInLane(blocked);

            // Ensure at least one safe lane
            OneSafeLane(blocked);

            float deltaCoinPosition = ChunkLength / coinLinePerChunk;
            // 2) Coins via factory on SAFE lanes
            //SpawnCoinsInLane(blocked, deltaCoinPosition);
        }

        private void OneSafeLane(bool[] blocked)
        {
            if (ensureAtLeastOneSafeLane)
            {
                bool anySafe = false;
                for (int i = 0; i < blocked.Length; i++) if (!blocked[i]) { anySafe = true; break; }
                if (!anySafe)
                {
                    // Pick a lane to clear – remove first obstacle child closest to that lane
                    int idx = Random.Range(0, blocked.Length);
                    // Children can be obstacles or coins; we only clear obstacles here
                    foreach (Transform child in transform)
                    {
                        if ((child.position - lanePoints[idx].position).sqrMagnitude < 0.01f)
                        {
                            var hazard = child.GetComponent<ObstacleHazard>();
                            if (hazard != null)
                            {
                                // Find an obstacle factory to recycle it (any is fine if they share the same pool type)
                                foreach (var f in obstacleFactories)
                                {
                                    if (f) { f.Recycle(hazard); break; }
                                }
                                blocked[idx] = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void SpawnObstaclesInLane(bool[] blocked)
        {
            for (int i = 0; i < lanePoints.Length; i++)
            {
                if (blocked[i] || obstacleFactories == null) continue;

                if (Random.value <= obstacleChancePerLane)
                {
                    var f = obstacleFactories[Random.Range(0, obstacleFactories.Length)];
                    if (f)
                    {
                        f.Spawn(transform, lanePoints[i].position, lanePoints[i].rotation, true);
                        blocked[i] = true;
                    }
                }
            }
        }
        
        private void SpawnCoinsInLane(bool[] blocked, float deltaCoinPosition)
        {
            for (int i = 0; i < lanePoints.Length; i++)
            {
                if (blocked[i] || !coinFactory) continue;

                for (int j = 0; j < coinLinePerChunk; j++) // try twice per lane
                {
                    if (Random.value <= coinChancePerLane)
                    {
                        // Slight vertical offset so coin sits above ground
                        var pos = lanePoints[i].position + Vector3.up * 1.5f + deltaCoinPosition * j * Vector3.forward;
                        CoinCollectible thiscoin = coinFactory.Spawn( pos, lanePoints[i].rotation);
                        thiscoin.transform.SetParent(transform, true);
                    }
                }
            }
        }
    }
}
