using MagicVillageDash.Collectibles;
using MagicVillageDash.Obstacles;
using UnityEngine;

namespace MagicVillageDash.Obstacles
{
    public sealed class ObstacleRaidFiller : MonoBehaviour
    {
        [Header("Lane spawn points inside this chunk (center of lanes/rows)")]
        [SerializeField] Transform[] lanePoints;

        [Header("Factories")]
        [Tooltip("One or more obstacle factories; a random one will be used.")]
        [SerializeField] private ObstacleFactory[] obstacleFactories;
        private CoinFactory coinFactory;

        [Header("Rules")]
        [Range(0, 1)][SerializeField] float obstacleChancePerLane = 0.45f;
        [Range(1, 5)][SerializeField] private int obstacleLinePerChunk = 1;
        [SerializeField] bool ensureAtLeastOneSafeLane = true;
        public float ChunkLength { get; set; } = 40f;


        public void Spawn()
        {
            if (lanePoints == null || lanePoints.Length == 0) return;

            bool[] blocked = new bool[lanePoints.Length];
            float deltaObstaclePosition = ChunkLength / obstacleLinePerChunk;
            // obstacles
            SpawnObstaclesInLane(blocked, deltaObstaclePosition);

            // Ensure at least one safe lane
            OneSafeLane(blocked);
            
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
        private void SpawnObstaclesInLane(bool[] blocked, float deltaObstaclePosition)
        {
            for (int i = 0; i < lanePoints.Length; i++)
            {
                if (blocked[i] || obstacleFactories == null) continue;

                for (int j = 0; j < obstacleLinePerChunk; j++) // try twice per lane
                {
                    if (Random.value <= obstacleChancePerLane)
                    {
                        var f = obstacleFactories[Random.Range(0, obstacleFactories.Length)];
                        if (f)
                        {
                            var pos = lanePoints[i].position + deltaObstaclePosition * j * Vector3.forward;
                            f.Spawn(transform, pos, lanePoints[i].rotation, true);
                            blocked[i] = true;
                        }
                    }
                }
            }
        }
    }
}
