using ErccDev.Foundation.Bootstrap;
using MagicVillageDash.FireBaseScripts;
using UnityEngine;

namespace MagicVillageDash.Bootstrap
{
    public sealed class LogoSceneController : SplashScreenControllerBase
    {
        protected override void LoadNow()
        {
            FirebaseAnalyticsService.Instance.LogAppStart();
            base.LoadNow();
        }
    }
}