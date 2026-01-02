using UnityEngine;

namespace MagicVillageDash.Obstacles
{
    /// <summary>
    /// Generates obstacles for a given Z range. Chunks call FillRange(...) to spawn
    /// obstacles that fall within their Z span.
    /// </summary>
    public sealed class ObstacleRailGenerator : MonoBehaviour
    {
        [Header("Factories")]
        [Tooltip("One or more obstacle factories; a random one will be used.")]
        [SerializeField] private ObstacleFactory[] obstacleFactories;

        [Header("Lanes (global, symmetric)")]
        [SerializeField, Min(2)] private int laneCount = 3;
        [SerializeField] private float laneWidth = 2.5f;  // distance between lane centers
        [SerializeField] private float laneCenterX = 0f;  // middle lane X

        [Header("Rules")]
        [Range(0, 1)][SerializeField] private float obstacleChancePerLane = 0.226f;
        [Range(1, 5)][SerializeField] private int obstacleLinePerChunk = 2;
        [SerializeField] private bool ensureAtLeastOneSafeLane = true;
        
        public void FillRange(Transform parent, float chunkLength, float zStart, float zEnd)
        {
            if (laneCount <= 0) return;

            bool[] blocked = new bool[laneCount];
            float deltaObstaclePosition = chunkLength / obstacleLinePerChunk;

            SpawnObstacles(parent, blocked, deltaObstaclePosition, zStart);

            if (ensureAtLeastOneSafeLane)
                EnsureOneSafeLane(parent, blocked);
        }

        void SpawnObstacles(Transform parent, bool[] blocked, float deltaObstaclePosition, float zStart)
        {
            for (int i = 0; i < laneCount; i++)
            {
                if (blocked[i] || obstacleFactories == null) continue;

                for (int j = 0; j < obstacleLinePerChunk; j++)
                {
                    if (Random.value <= obstacleChancePerLane)
                    {
                        var obstacleFactory = obstacleFactories[Random.Range(0, obstacleFactories.Length)];
                        if (obstacleFactory)
                        {
                            float x = LaneX(i);
                            float z = zStart + deltaObstaclePosition * j;
                            var pos = new Vector3(x, parent.position.y, z);

                            obstacleFactory.Spawn(parent, pos, Quaternion.identity, true);
                            blocked[i] = true;
                        }
                    }
                }
            }
        }

        void EnsureOneSafeLane(Transform parent, bool[] blocked)
        {
            if (HasAnySafeLane(blocked)) return;

            int idx = Random.Range(0, blocked.Length);
            var hazard = FindHazardClosestToLaneX(parent, idx);
            if (hazard == null) return;

            RecycleHazard(hazard);
            blocked[idx] = false;
        }

        bool HasAnySafeLane(bool[] blocked)
        {
            for (int i = 0; i < blocked.Length; i++)
                if (!blocked[i]) return true;
            return false;
        }

        ObstacleHazard FindHazardClosestToLaneX(Transform parent, int laneIndex)
        {
            float x = LaneX(laneIndex);

            float bestSqr = float.PositiveInfinity;
            ObstacleHazard best = null;

            foreach (Transform child in parent)
            {
                var hazard = child.GetComponent<ObstacleHazard>();
                if (hazard == null) continue;

                float dx = child.position.x - x;
                float sqr = dx * dx;

                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = hazard;
                }
            }

            return best;
        }

        void RecycleHazard(ObstacleHazard hazard)
        {
            if (obstacleFactories == null) return;

            foreach (var obstacleFactory in obstacleFactories)
            {
                if (obstacleFactory)
                {
                    obstacleFactory.Recycle(hazard);
                    break;
                }
            }
        }

        int MidLane() => Mathf.Clamp(laneCount / 2, 0, laneCount - 1);

        float LaneX(int laneIndex)
        {
            int mid = MidLane();
            int delta = laneIndex - mid;
            return laneCenterX + delta * laneWidth;
        }
    
    }
}
