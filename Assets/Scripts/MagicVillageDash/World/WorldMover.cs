using UnityEngine;

namespace MagicVillageDash.World
{
    /// <summary>Moves its transform along -Z using GameSpeedController.</summary>
    public sealed class WorldMover : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour speedSourceProvider;
        [Tooltip("Optional extra speed added/subtracted from global (e.g., conveyor).")]
        [SerializeField] private float forwardOffsetSpeed = 0f;
        IGameSpeedController speedSource;

        void Awake()
        {
            speedSource = speedSourceProvider as IGameSpeedController ?? speedSourceProvider.GetComponent<IGameSpeedController>();
        }

        void FixedUpdate()
        {
            if (speedSource == null) return;
            float speed = speedSource.CurrentSpeed + forwardOffsetSpeed;
            transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
        }
    }
}