using System;
using UnityEngine;
using MagicVillageDash.World;
using UnityEngine.Events;

namespace MagicVillageDash.Score
{
    /// <summary>Integrates world speed to meters. Player is stationary, world moves on -Z.</summary>
    public sealed class DistanceTracker : MonoBehaviour, IDistanceTracker 
    {
        [SerializeField] private MonoBehaviour gameSpeedProvider;
        [SerializeField] private bool autoRun = true;

        public float CurrentDistance { get; private set; }
        public bool  IsRunning { get; private set; }

        IGameSpeedController gameSpeedController;

        void Awake()
        {
            gameSpeedController = gameSpeedProvider as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
        }

        void Start()
        {
            if (autoRun) StartRun();
        }

        void Update()
        {
            if (!IsRunning || gameSpeedController == null) return;
            CurrentDistance += gameSpeedController.CurrentSpeed * 0.1f * Time.deltaTime;
        }

        public void StartRun()
        {
            IsRunning = true;
        }

        public void StopRun()
        {
            IsRunning = false;
        }

        public void ResetDistance()
        {
            CurrentDistance = 0f;
        }
    }
}
