using System;
using MagicVillageDash.Character;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemySensors : MonoBehaviour
    {
        public event Action<string> OnTriggerHit;
        void OnTriggerEnter(Collider other)
        {            
            OnTriggerHit?.Invoke(other.tag);
        }
    }
}
