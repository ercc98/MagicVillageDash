using System;
using UnityEngine;
using MagicVillageDash.World;
using UnityEngine.Events;

namespace MagicVillageDash.Score
{
    /// <summary>Integrates world speed to meters. Player is stationary, world moves on -Z.</summary>
    public sealed class DistanceTracker : MonoBehaviour
    {
        [SerializeField] private GameSpeedController speedSource;
        [SerializeField] private bool autoRun = true;

        public float DistanceMeters { get; private set; }
        public bool  IsRunning { get; private set; }

        public event Action<float> DistanceChanged; // typed event

        [SerializeField] private UnityEvent<float> onDistanceChanged; // for UI via Inspector

        void Awake()
        {
            if (!speedSource) speedSource = FindAnyObjectByType<GameSpeedController>();
        }

        void Start()
        {
            if (autoRun) StartRun();
        }

        void Update()
        {
            if (!IsRunning || !speedSource) return;
            DistanceMeters += speedSource.CurrentSpeed * Time.deltaTime;
            DistanceChanged?.Invoke(DistanceMeters);
            onDistanceChanged?.Invoke(DistanceMeters);
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
            DistanceMeters = 0f;
            DistanceChanged?.Invoke(DistanceMeters);
            onDistanceChanged?.Invoke(DistanceMeters);
        }
    }
}