using System;
using UnityEngine;

namespace MagicVillageDash.World
{
    public interface IGameSpeedController
    {
        float CurrentSpeed { get; }
        float MaxSpeed { get; }
        void SetSpeed(float value);
        void ResetSpeed();
    }
}
