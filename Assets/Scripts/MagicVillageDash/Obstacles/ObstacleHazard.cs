using System;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Obstacles
{
    [RequireComponent(typeof(Collider))]
    public sealed class ObstacleHazard : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [Tooltip("Auto disable on hit to avoid double-trigger; factory will recycle.")]
        public ChunkRoot Owner { get; internal set; }

        /// <summary>Raised when the player hits the obstacle.</summary>
        public event Action<ObstacleHazard, GameObject> Hit;

        void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true; // endless runners usually use trigger hazards
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            Hit?.Invoke(this, other.gameObject);
            gameObject.SetActive(false);
        }
    }
}