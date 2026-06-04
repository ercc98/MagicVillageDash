using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using ErccDev.Foundation.Core.Events;

namespace MagicVillageDash.Achievements.Rewards
{
    /// <summary>
    /// Decoupled reward: fires an EventBus event so the economy/UI can react to an unlock
    /// (e.g. credit coins, show a banner) without the achievements layer owning that logic.
    /// </summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Achievements/Rewards/Event Bus Reward")]
    public sealed class EventBusReward : Reward
    {
        [SerializeField] private string eventName = "achievementReward";
        [SerializeField] private int    amount    = 0;

        public override void Grant(IAchievementContext context)
        {
            EventBus.Trigger(eventName, new()
            {
                ["rewardId"] = rewardId,
                ["amount"]   = amount,
            });
        }
    }
}
