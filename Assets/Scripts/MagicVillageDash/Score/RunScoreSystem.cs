using System;
using UnityEngine;
using ErccDev.Foundation.Core.Save; // SaveService

namespace MagicVillageDash.Score
{
    /// <summary>Aggregates coins + distance into a single score.</summary>
    public sealed class RunScoreSystem : MonoBehaviour, IRunScoreSystem
    {
        [Header("Sources")]
        [SerializeField] private MonoBehaviour distanceTrackerProvider;
        [SerializeField] private MonoBehaviour coinCounterProvider;

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

        public event Action<int>   OnScoreChanged;
        public event Action<int>   OnBestScoreChanged;
        public event Action<float> OnBestDistanceChanged;

        const string kSaveFile = "run_stats.json";

        IDistanceTracker distanceTracker;
        ICoinCounter     coinCounter;

        [Serializable]
        private class RunStatsData
        {
            public int   bestScore;
            public float bestDistanceMeters;
            public int   bestCoins;
        }

        void Awake()
        {
            distanceTracker = distanceTrackerProvider   as IDistanceTracker     ?? FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
            coinCounter     = coinCounterProvider       as ICoinCounter         ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);

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
            if (coinCounter    != null) coinCounter.CoinsChanged       += OnCoinsChanged;
        }

        void OnDisable()
        {
            if (coinCounter != null) coinCounter.CoinsChanged -= OnCoinsChanged;
        }

        void Update()
        {
            RecomputeScore(distanceTracker.CurrentDistance);
        }

        void RecomputeScore(float meters)
        {
            int metersPoints = (int)meters * pointsPerMeter;
            int coinPoints   = (coinCounter != null ? coinCounter.Coins : 0) * pointsPerCoin;
            int newScore     = metersPoints + coinPoints;

            if (newScore != currentScore)
            {
                currentScore = newScore;
                OnScoreChanged?.Invoke(currentScore);
            }
        }

        void OnCoinsChanged(int _)
        {
            RecomputeScore(distanceTracker != null ? distanceTracker.CurrentDistance : 0f);
        }

        /// <summary>Call at Game Over to save bests.</summary>
        public void CommitIfBest()
        {
            float dist = distanceTracker != null ? distanceTracker.CurrentDistance : 0f;
            int   c    = coinCounter != null ? coinCounter.Coins : 0;

            bool changed = false;

            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                changed = true;
                OnBestScoreChanged?.Invoke(bestScore);
            }

            if (dist > bestDistanceMeters)
            {
                bestDistanceMeters = dist;
                changed = true;
                OnBestDistanceChanged?.Invoke(bestDistanceMeters);
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
            OnScoreChanged?.Invoke(currentScore);
            coinCounter?.ResetCoins(0);
            distanceTracker?.ResetDistance();
        }
    }
}
