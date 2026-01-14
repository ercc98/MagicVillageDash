using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

namespace MagicVillageDash.AdMobScripts
{
    public class Interstitial : MonoBehaviour
    {
        public static Interstitial Instance { get; private set; }
        private InterstitialAd interstitialAd;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        public void Start()
        {
            // Initialize Google Mobile Ads Unity Plugin.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                RequestInterstitial();
            });
            

            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                RegisterEventHandlers();
                interstitialAd.Show();
            }
        }
        public void RequestInterstitial()
        {
            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            InterstitialAd.Load("ca-app-pub-3940256099942544/1033173712", adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load with error: " + error.GetMessage());
                    // The ad failed to load.
                    return;
                }
                // The ad loaded successfully.
                interstitialAd = ad;
            });
            
        }

        public void RegisterEventHandlers()
        {
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            interstitialAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                // Raised when the ad closed full screen content.
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                // Raised when the ad failed to open full screen content.
            };
        }

        void OnDestroy()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
        }


    }
}
