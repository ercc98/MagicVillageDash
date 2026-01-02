using ErccDev.Foundation.Core.Factories;
using UnityEngine;

namespace MagicVillageDash.Collectibles
{

    public sealed class CoinRailGenerator : MonoBehaviour
    {
        [Header("Factory")]
        [SerializeField] private CoinFactory coinFactoryProvider;
        IFactory<CoinCollectible> iCoinFactory;

        [Header("Lanes (global, symmetric)")]
        [SerializeField, Min(2)] private int laneCount = 3;
        [SerializeField] private float laneWidth = 2.5f;  // distance between lane centers
        [SerializeField] private float laneCenterX = 0f;  // middle lane X

        [Header("Rail Shape")]
        [SerializeField] private float coinSpacingZ = 1.8f;    // distance between coins along Z
        [SerializeField] private float minStraightZ = 8f;      // min straight distance before a turn
        [SerializeField] private float maxStraightZ = 18f;     // max straight distance before a turn
        [SerializeField] private float transitionLengthZ = 12f;// how long to move from lane A to B
        [SerializeField] private float coinHeight = 1.0f;      // Y position for coins

        // ---- Runtime state (persistent across chunk fills) ----
        float _lastCoinZ = float.NegativeInfinity;
        bool  _initialized;
        int   _currentLane;    // 0..laneCount-1
        int   _targetLane;     // next lane during transition
        float _segStartZ;      // current segment start Z
        float _segEndZ;        // current segment end Z
        SegmentType _segment;

        enum SegmentType { Straight, Transition }

        // ------------- Public API -------------

        /// <summary>Call when a new run starts, giving the Z to start generating from (e.g., player.z).</summary>
        public void ResetPathAt(float startZ)
        {
            _initialized  = false;
            _lastCoinZ    = startZ - coinSpacingZ; // so first coin can spawn near start
            _currentLane  = MidLane();
            _targetLane   = _currentLane;
            _segStartZ    = startZ;
            _segEndZ      = startZ;
            _segment      = SegmentType.Straight;
        }

        /// <summary>
        /// Spawn coins for the given Z range into the given parent (usually the chunk transform).
        /// Assumes chunks are created in increasing Z order.
        /// </summary>
        public void FillRange(Transform parent, float zStart, float zEnd)
        {
            
            if (!_initialized) InitializeAt(zStart);

            // Ensure monotonic coin Z
            float z = Mathf.Max(zStart, _lastCoinZ + coinSpacingZ);
            z = zStart;
            PlanNextSegment();
            while (z <= zEnd)
            {

                // Advance to next segment(s) if necessary
                while (z > _segEndZ)
                    PlanNextSegment();
                

                // Compute X at this Z
                float x = (_segment == SegmentType.Straight)
                    ? LaneX(_currentLane)
                    : Mathf.Lerp(LaneX(_currentLane), LaneX(_targetLane), Mathf.InverseLerp(_segStartZ, _segEndZ, z));

                // Spawn coin at (x, coinHeight, z), parented under the chunk (worldSpace: true)
                iCoinFactory.Spawn( new Vector3(x, coinHeight, z), Quaternion.identity, parent);
                _lastCoinZ = z;
                z += coinSpacingZ;
            }
        }

        // ------------- Internals -------------

        void InitializeAt(float startZ)
        {
            _currentLane = MidLane();
            _targetLane  = _currentLane;
            _segment     = SegmentType.Straight;
            _segStartZ   = startZ;
            _segEndZ     = startZ + Random.Range(minStraightZ, maxStraightZ);
            _initialized = true;
            if (_lastCoinZ == float.NegativeInfinity)
                _lastCoinZ = startZ - coinSpacingZ;

            iCoinFactory = coinFactoryProvider as IFactory<CoinCollectible> ?? FindAnyObjectByType<CoinFactory>(FindObjectsInactive.Exclude);
        }

        void PlanNextSegment()
        {
            if (_segment == SegmentType.Straight)
            {
                // Begin a transition to adjacent lane
                int to = ChooseAdjacentLane(_currentLane);
                _segment = SegmentType.Transition;
                _segStartZ = _segEndZ;
                _segEndZ = _segStartZ + transitionLengthZ;
                _targetLane = to;
            }
            else
            {
                // Finish transition -> new straight
                _currentLane = _targetLane;
                _segment = SegmentType.Straight;
                _segStartZ = _segEndZ;
                _segEndZ = _segStartZ + Random.Range(minStraightZ, maxStraightZ);
            }
        }

        int ChooseAdjacentLane(int from)
        {
            // Prefer moving inward if at edges; otherwise pick left/right randomly
            if (from <= 0)                 return from + 1;
            if (from >= laneCount - 1)     return from - 1;
            return Random.value < 0.5f ? from - 1 : from + 1;
        }

        int MidLane() => Mathf.Clamp(laneCount / 2, 0, laneCount - 1);

        float LaneX(int laneIndex)
        {
            // 0..N-1 => symmetric around laneCenterX
            int mid = MidLane();
            int delta = laneIndex - mid;
            return laneCenterX + delta * laneWidth;
        }
    }
}
