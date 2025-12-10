using ErccDev.Foundation.Audio;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Score;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Runner
{
    public sealed class RunController : GameSessionController
    {
        [Header("Dependencies")]
        [SerializeField] private MonoBehaviour scoreProvider;
        [SerializeField] private MonoBehaviour distanceProvider;
        [SerializeField] private MonoBehaviour coinsProvider;
        [SerializeField] private MonoBehaviour speedProvider;

        IRunScoreSystem      score;
        IDistanceTracker     distance;
        ICoinCounter         coins;
        IGameSpeedController speed;

        protected override void Awake()
        {
            base.Awake();
            score = scoreProvider as IRunScoreSystem;
            distance = distanceProvider as IDistanceTracker     ?? FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
            coins    = coinsProvider    as ICoinCounter         ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
            speed    = speedProvider    as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
        }

        protected override void ResetSessionState()
        {
            score?.ResetRun();
            distance?.ResetDistance();
            coins?.ResetCoins(0);
            speed?.ResetSpeed();
        }

        protected override void OnSessionStarted()
        {
            AudioService?.Play("run_start", SoundCategory.SFX);
        }

        protected override void OnSessionEnded()
        {
            score?.CommitIfBest();
            speed?.SetSpeed(0f);
            AudioService?.Play("game_over", SoundCategory.SFX);
        }
    }
}
