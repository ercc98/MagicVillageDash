using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using MagicVillageDash.Score;

namespace MagicVillageDash.Achievements.Conditions
{
    /// <summary>Unlocks when the current run reaches <see cref="targetMeters"/> meters.</summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Achievements/Conditions/Distance Reached")]
    public sealed class DistanceReachedCondition : AchievementCondition
    {
        [Min(1f)] [SerializeField] private float targetMeters = 1000f;

        private IDistanceTracker _distance;

        public override void Initialize(IAchievementContext context) => context?.TryGet(out _distance);

        public override bool IsCompleted() => _distance != null && _distance.CurrentDistance >= targetMeters;

        public override float Progress01 =>
            _distance == null ? 0f : Mathf.Clamp01(_distance.CurrentDistance / targetMeters);

        public override void Cleanup() => _distance = null;
    }
}
