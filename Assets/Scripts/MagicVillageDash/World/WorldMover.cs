using UnityEngine;

namespace MagicVillageDash.World
{
    /// <summary>Moves its transform along -Z using GameSpeedController.</summary>
    public sealed class WorldMover : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour speedSourceProvider;
        
        IGameSpeedController speedSource;

        void Awake()
        {
            speedSource = speedSourceProvider as IGameSpeedController ?? speedSourceProvider.GetComponent<IGameSpeedController>();
        }

        void FixedUpdate()
        {
            if (speedSource == null) return;
            float speed = speedSource.CurrentSpeed;
            transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
            //RecenterIfNeeded();
        }

        void RecenterIfNeeded()
        {
            //Need to implement!
        }
    }
}