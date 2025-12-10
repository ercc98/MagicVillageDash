using UnityEngine;

namespace MagicVillageDash.World
{
    /// <summary>Holds the forward world speed. Everyone reads from here.</summary>
    public sealed class GameSpeedController : MonoBehaviour, IGameSpeedController
    {
        [SerializeField] private float baseSpeed = 8f;
        [SerializeField] private float accelerationPerSecond = 0.15f;
        [SerializeField] private float maxSpeed = 30f;

        public float CurrentSpeed { get; private set; }

        void Awake() => CurrentSpeed = baseSpeed;

        void Update()
        {
            if (CurrentSpeed < maxSpeed)
                CurrentSpeed = Mathf.Min(maxSpeed, CurrentSpeed + accelerationPerSecond * Time.deltaTime);
        }

        public void SetSpeed(float value) => CurrentSpeed = Mathf.Max(0f, value);
        public void ResetSpeed() => CurrentSpeed = baseSpeed;
    }
}
