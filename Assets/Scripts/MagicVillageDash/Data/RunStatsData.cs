using UnityEngine;

namespace MagicVillageDash.Data
{
    [CreateAssetMenu(menuName = "MagicVillageDash/Data/Run Stats", fileName = "RunStatsData")]
    public sealed class RunStatsData : ScriptableObject
    {
        [Header("Best Records")]
        public int bestScore = 0;
        public float bestDistance = 0;
        public int bestCoins = 0;

        [Header("Last Run (optional)")]
        public int lastScore;
        public float lastDistanceMeters;
        public int lastCoins;

        public void RegisterRun(int score, float distanceMeters, int coins)
        {
            lastScore = score;
            lastDistanceMeters = distanceMeters;
            lastCoins = coins;

            if (score > bestScore) bestScore = score;
            if (distanceMeters > bestDistance) bestDistance = distanceMeters;
            if (coins > bestCoins) bestCoins = coins;
        }

        public void ResetAll()
        {
            bestScore = 0;
            bestDistance = 0;
            bestCoins = 0;
            lastScore = 0;
            lastDistanceMeters = 0;
            lastCoins = 0;
        }
    }
}
