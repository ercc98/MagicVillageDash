using System.Collections.Generic;
using UnityEngine;

namespace MagicVillageDash.Data
{
    /// <summary>
    /// Persistent unlock state for achievements. Mirrors RunStatsData / SettingsData and is saved
    /// through GameDataService's ScriptableObject list (playerdata.json) rather than its own file.
    /// Only earned ids are stored — live progress is recomputed from each achievement's condition.
    /// </summary>
    [CreateAssetMenu(menuName = "MagicVillageDash/Data/Achievement Data", fileName = "AchievementData")]
    public sealed class AchievementData : ScriptableObject
    {
        [Header("Unlocked")]
        [SerializeField] private List<string> unlockedIds = new();

        [Header("Last Unlock (optional)")]
        [SerializeField] private string lastUnlockedId;

        public IReadOnlyList<string> UnlockedIds => unlockedIds;
        public int    Count          => unlockedIds.Count;
        public string LastUnlockedId => lastUnlockedId;

        public bool IsUnlocked(string id)
            => !string.IsNullOrEmpty(id) && unlockedIds.Contains(id);

        /// <summary>Adds an id if new (and notes it as the latest). True when actually added.</summary>
        public bool MarkUnlocked(string id)
        {
            if (string.IsNullOrEmpty(id) || unlockedIds.Contains(id)) return false;
            unlockedIds.Add(id);
            lastUnlockedId = id;
            return true;
        }

        /// <summary>Replaces the stored set from the manager's authoritative unlock list.</summary>
        public void Sync(IEnumerable<string> ids)
        {
            unlockedIds.Clear();
            foreach (var id in ids)
                if (!string.IsNullOrEmpty(id) && !unlockedIds.Contains(id))
                    unlockedIds.Add(id);
        }

        public void ResetAll()
        {
            unlockedIds.Clear();
            lastUnlockedId = null;
        }
    }
}
