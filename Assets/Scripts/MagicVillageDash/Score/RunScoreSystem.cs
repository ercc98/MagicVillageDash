using System;
using UnityEngine;
using ErccDev.Foundation.Core.Save; // SaveService

namespace MagicVillageDash.Score
{
    /// <summary>Aggregates coins + distance into a single score.</summary>
    public sealed class RunScoreSystem : MonoBehaviour, IRunScoreSystem
    {
        [Header("Sources")]
        [SerializeField] private MonoBehaviour distanceProvider;
        [SerializeField] private MonoBehaviour coinsProvider;

        [Header("Scoring")]
        [Tooltip("Points per whole meter traveled.")]
        [SerializeField] private int pointsPerMeter = 1;
        [Tooltip("Points per coin collected.")]
        [SerializeField] private int pointsPerCoin  = 10;

        [Header("Runtime (read-only)")]
        [SerializeField] private int   currentScore;
        [SerializeField] private int   bestScore;
        [SerializeField] private float bestDistanceMeters;
        [SerializeField] private int   bestCoins;

        public int   CurrentScore       => currentScore;
        public int   BestScore          => bestScore;
        public float BestDistanceMeters => bestDistanceMeters;
        public int   BestCoins          => bestCoins;

        public event Action<int>   ScoreChanged;
        public event Action<int>   BestScoreChanged;
        public event Action<float> BestDistanceChanged;

        [SerializeField] private Action<int>   onScoreChanged;
        [SerializeField] private Action<int>   onBestScoreChanged;
        [SerializeField] private Action<float> onBestDistanceChanged;

        const string kSaveFile = "run_stats.json";

        IDistanceTracker distance;
        ICoinCounter     coins;

        [Serializable]
        private class RunStatsData
        {
            public int   bestScore;
            public float bestDistanceMeters;
            public int   bestCoins;
        }

        void Awake()
        {
            distance = distanceProvider as IDistanceTracker
                       ?? FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
            coins    = coinsProvider    as ICoinCounter
                       ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);

            // Load bests
            if (SaveService.TryLoadObject(kSaveFile, out RunStatsData data))
            {
                bestScore          = data.bestScore;
                bestDistanceMeters = data.bestDistanceMeters;
                bestCoins          = data.bestCoins;
            }
        }

        void OnEnable()
        {
            if (distance != null) distance.DistanceChanged += RecomputeScore;
            if (coins    != null) coins.CoinsChanged       += OnCoinsChanged;
        }

        void OnDisable()
        {
            if (distance != null) distance.DistanceChanged -= RecomputeScore;
            if (coins    != null) coins.CoinsChanged       -= OnCoinsChanged;
        }

        void RecomputeScore(float meters)
        {
            int metersPoints = Mathf.FloorToInt(meters) * pointsPerMeter;
            int coinPoints   = (coins != null ? coins.Coins : 0) * pointsPerCoin;
            int newScore     = metersPoints + coinPoints;

            if (newScore != currentScore)
            {
                currentScore = newScore;
                ScoreChanged?.Invoke(currentScore);
                onScoreChanged?.Invoke(currentScore);
            }
        }

        void OnCoinsChanged(int _)
        {
            RecomputeScore(distance != null ? distance.DistanceMeters : 0f);
        }

        /// <summary>Call at Game Over to save bests.</summary>
        public void CommitIfBest()
        {
            float dist = distance != null ? distance.DistanceMeters : 0f;
            int   c    = coins != null ? coins.Coins : 0;

            bool changed = false;

            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                changed = true;
                BestScoreChanged?.Invoke(bestScore);
                onBestScoreChanged?.Invoke(bestScore);
            }

            if (dist > bestDistanceMeters)
            {
                bestDistanceMeters = dist;
                changed = true;
                BestDistanceChanged?.Invoke(bestDistanceMeters);
                onBestDistanceChanged?.Invoke(bestDistanceMeters);
            }

            if (c > bestCoins)
            {
                bestCoins = c;
                changed = true;
            }

            if (changed)
            {
                var data = new RunStatsData
                {
                    bestScore = bestScore,
                    bestDistanceMeters = bestDistanceMeters,
                    bestCoins = bestCoins
                };
                SaveService.SaveObject(data, kSaveFile);
            }
        }

        public void ResetRun()
        {
            currentScore = 0;
            ScoreChanged?.Invoke(currentScore);
            onScoreChanged?.Invoke(currentScore);
            coins?.ResetCoins(0);
            distance?.ResetDistance();
        }
    }
}
