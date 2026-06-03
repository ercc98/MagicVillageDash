using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Data;
using MagicVillageDash.Score;

namespace MagicVillageDash.Achievements
{
    /// <summary>
    /// Game-side achievement engine. Subclasses Foundation's <see cref="AchievementManagerBase"/>
    /// (all content lives in the SO definitions) and re-evaluates pending conditions when something
    /// relevant happens: coins change mid-run, and the run ends. Hidden popups/SFX hook into
    /// <see cref="AchievementManagerBase.OnUnlocked"/>.
    ///
    /// Persistence is routed through <see cref="AchievementData"/> in GameDataService's save list
    /// (playerdata.json) instead of the base's standalone "achievements" file.
    /// </summary>
    public sealed class AchievementManager : AchievementManagerBase
    {
        [Header("Persistence")]
        [SerializeField] private AchievementData achievementData;

        private ICoinCounter _coinCounter;

        protected override void Awake()
        {
            // Feed the base its live context: the builder doubles as an IAchievementContext.
            contextProvider ??= FindAnyObjectByType<AchievementContextBuilder>(FindObjectsInactive.Exclude);
            base.Awake();   // base.Awake() calls LoadProgress() below

            _coinCounter = FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
        }

        // ---------- Persistence routed through the SO save system ----------

        protected override void LoadProgress()
        {
            if (achievementData == null) return;

            GameDataService._instance?.LoadAll();   // pull latest playerdata.json into the SOs
            foreach (var id in achievementData.UnlockedIds)
                if (!string.IsNullOrEmpty(id)) _unlocked.Add(id);
        }

        protected override void SaveProgress()
        {
            if (achievementData == null) return;

            achievementData.Sync(_unlocked);        // mirror the authoritative set into the SO
            GameDataService._instance?.SaveAll();   // persist the whole playerdata.json
        }

        protected override void Unlock(AchievementDefinition def)
        {
            // Note the latest id before the base persists, so it lands in the same save.
            if (def != null && achievementData != null && !IsUnlocked(def.achievementId))
                achievementData.MarkUnlocked(def.achievementId);

            base.Unlock(def);   // adds to _unlocked, grants rewards, calls SaveProgress(), fires OnUnlocked
        }

        private void OnEnable()
        {
            GameEvents.GameOver += Evaluate;
            if (_coinCounter != null) _coinCounter.CoinsChanged += OnCoinsChanged;
            OnUnlocked += HandleUnlocked;
        }

        private void OnDisable()
        {
            GameEvents.GameOver -= Evaluate;
            if (_coinCounter != null) _coinCounter.CoinsChanged -= OnCoinsChanged;
            OnUnlocked -= HandleUnlocked;
        }

        private void OnCoinsChanged(int _) => Evaluate();

        private void HandleUnlocked(AchievementDefinition def)
        {
            // Base already persists, grants rewards, and fires the "achievementUnlocked" EventBus
            // event for UI. Plug a toast/SFX here when the achievements HUD lands.
            Debug.Log($"[Achievements] Unlocked: {def.title} ({def.achievementId})", this);
        }
    }
}
