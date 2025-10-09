using UnityEngine;

namespace MagicVillageDash.World
{
    /// <summary>Moves its transform along -Z using GameSpeedController.</summary>
    public sealed class WorldMover : MonoBehaviour
    {
        [SerializeField] private GameSpeedController speedSource;
        [Tooltip("Optional extra speed added/subtracted from global (e.g., conveyor).")]
        [SerializeField] private float forwardOffsetSpeed = 0f;

        void Awake()
        {
            if (!speedSource) speedSource = FindAnyObjectByType<GameSpeedController>();
        }

        void FixedUpdate()
        {
            if (!speedSource) return;
            float speed = speedSource.CurrentSpeed + forwardOffsetSpeed;
            transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
        }
    }
}