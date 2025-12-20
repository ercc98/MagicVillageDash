using Firebase.Extensions; 
using UnityEngine;
namespace MagicVillageDash.FireBaseScripts
{
    public class FirebaseAnalyticsService : MonoBehaviour
    {
        public static FirebaseAnalyticsService Instance;
        private bool isFirebaseReady = false;
        private Firebase.FirebaseApp app;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    app = Firebase.FirebaseApp.DefaultInstance;
                    isFirebaseReady = true;
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        public void LogAppStart()
        {
            if (!isFirebaseReady) return;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("app_start");
        }

        public void LogStartPlaying()
        {
            if (!isFirebaseReady) return;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("start_playing");
            
        }

        public void LogPlayerDied(float distance, int coins)
        {
            if (!isFirebaseReady) return;

            var parameters = new System.Collections.Generic.List<Firebase.Analytics.Parameter>
            {
                new("distance", distance),
                new("coins", coins)
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("player_died", parameters.ToArray());

        }

        public void LogStopPlaying()
        {
            if (!isFirebaseReady) return;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("stop_playing");
        }
    }
}