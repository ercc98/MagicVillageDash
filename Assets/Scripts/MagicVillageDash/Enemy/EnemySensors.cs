using System;
using MagicVillageDash.Character;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemySensors : MonoBehaviour
    {
        public event Action<string> TriggerHit;
        void OnTriggerEnter(Collider other)
        {
            TriggerHit?.Invoke(other.tag);
        }
    }
}
