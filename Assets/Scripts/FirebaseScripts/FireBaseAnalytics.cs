using Firebase.Analytics;
using Firebase.Extensions; 
using Firebase; 
using UnityEngine;

    public class FireBaseAnalytics : MonoBehaviour
    {
        public static FireBaseAnalytics Instance;
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
    }
        
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        /*
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            app = Firebase.FirebaseApp.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
        */
    }
