using System;
using ErccDev.Foundation.Audio;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Audio;
using MagicVillageDash.Score;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Runner
{
    public sealed class RunController : GameSessionController
    {
        [Header("Dependencies")]
        [SerializeField] private MonoBehaviour runScoreSystemProvider;
        [SerializeField] private MonoBehaviour distanceTrackerProvider;
        [SerializeField] private MonoBehaviour coinCounterProvider;
        [SerializeField] private MonoBehaviour gameSpeedProvider;

        IRunScoreSystem      runScoreSystem;
        IDistanceTracker     distanceTracker;
        ICoinCounter         coinCounter;
        IGameSpeedController gameSpeedController;

        protected override void Awake()
        {
            base.Awake();
            runScoreSystem = runScoreSystemProvider as IRunScoreSystem ?? FindAnyObjectByType<RunScoreSystem>(FindObjectsInactive.Exclude);
            distanceTracker = distanceTrackerProvider as IDistanceTracker ?? FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
            coinCounter = coinCounterProvider as ICoinCounter ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
            gameSpeedController = gameSpeedProvider as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
            GameEvents.GameOver += OnGameOver;
            GameEvents.GameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            coinCounter?.ResetCoins(0);
            distanceTracker?.ResetDistance();
            gameSpeedController?.ResetSpeed();
            runScoreSystem?.ResetRun();
            distanceTracker?.StartRun();
            MagicVillageDashAudioManager.Instance?.Play(MusicId.GameTheme2);
        }


        private void OnGameOver()
        {            
            distanceTracker?.StopRun();
            runScoreSystem?.CommitIfBest();
            MagicVillageDashAudioManager.Instance?.StopLoop(SoundCategory.Music);
        }


        void OnDestroy()
        {
            GameEvents.GameOver   -= OnGameOver;
            GameEvents.GameStarted -= OnGameStarted;
        }

        protected override void ResetSessionState()
        {
            runScoreSystem?.ResetRun();
            distanceTracker?.ResetDistance();
            coinCounter?.ResetCoins(0);
            gameSpeedController?.ResetSpeed();
        }

        protected override void OnSessionStarted()
        {
            AudioService?.Play("run_start", SoundCategory.SFX);
        }

        protected override void OnSessionEnded()
        {
            runScoreSystem?.CommitIfBest();
            gameSpeedController?.SetSpeed(0f);
            AudioService?.Play("game_over", SoundCategory.SFX);
        }
    }
}
