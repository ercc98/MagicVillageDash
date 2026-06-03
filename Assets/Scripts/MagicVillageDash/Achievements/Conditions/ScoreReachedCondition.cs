using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using MagicVillageDash.Score;

namespace MagicVillageDash.Achievements.Conditions
{
    /// <summary>Unlocks when the current run's score reaches <see cref="targetScore"/>.</summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Achievements/Conditions/Score Reached")]
    public sealed class ScoreReachedCondition : AchievementCondition
    {
        [Min(1)] [SerializeField] private int targetScore = 5000;

        private IRunScoreSystem _score;

        public override void Initialize(IAchievementContext context) => context?.TryGet(out _score);

        public override bool IsCompleted() => _score != null && _score.CurrentScore >= targetScore;

        public override float Progress01 =>
            _score == null ? 0f : Mathf.Clamp01((float)_score.CurrentScore / targetScore);

        public override void Cleanup() => _score = null;
    }
}
