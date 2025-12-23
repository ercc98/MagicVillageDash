using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Save;

namespace MagicVillageDash.Data
{
    public sealed class GameDataService : GameDataServiceBase
    {
        [Header("Persistent Data")]
        [SerializeField] private PlayerProfileData playerProfile;
        [SerializeField] private RunStats runStats;
        //[SerializeField] private ScriptableObject progress;
        //[SerializeField] private ScriptableObject records;

        protected override void Awake()
        {
            base.Awake();
            // Ensure profile is valid on first run and gets persisted
            if (playerProfile)
            {
                playerProfile.EnsureInitialized();
                SaveAll(pretty: true);
            }
            if (runStats)
            {
                runStats.RegisterRun(0, 0, 0 );
            }
        }
        protected override List<ScriptableObject> BuildObjects()
        {
            return new List<ScriptableObject>
            {
                playerProfile,
                runStats,
                //settings,
                //progress,
                //records
            };
        }
    }
}
