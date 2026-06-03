using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using MagicVillageDash.Score;

namespace MagicVillageDash.Achievements.Conditions
{
    /// <summary>Unlocks when the player collects <see cref="targetCoins"/> in a single run.</summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Achievements/Conditions/Coins In Run")]
    public sealed class CoinsInRunCondition : AchievementCondition
    {
        [Min(1)] [SerializeField] private int targetCoins = 50;

        private ICoinCounter _coins;

        public override void Initialize(IAchievementContext context) => context?.TryGet(out _coins);

        public override bool IsCompleted() => _coins != null && _coins.Coins >= targetCoins;

        public override float Progress01 =>
            _coins == null ? 0f : Mathf.Clamp01((float)_coins.Coins / targetCoins);

        public override void Cleanup() => _coins = null;
    }
}
