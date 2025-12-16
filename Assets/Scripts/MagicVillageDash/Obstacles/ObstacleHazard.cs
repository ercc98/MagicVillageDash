using System;
using MagicVillageDash.Gameplay;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Obstacles
{
    [RequireComponent(typeof(Collider))]
    public sealed class ObstacleHazard : MonoBehaviour
    {
        [Tooltip("Auto disable on hit to avoid double-trigger; factory will recycle.")]
        public ChunkRoot Owner { get; internal set; }

        public event Action<ObstacleHazard> Hit;

        void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true; 
        }

        void OnTriggerEnter(Collider other)
        {
            
            if (other.TryGetComponent<IHazardReceiver>(out var r) )
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                r.OnHazardHit(hitPoint);
            }

            Hit?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}