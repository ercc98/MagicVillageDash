using System;

namespace MagicVillageDash.Score
{
    public interface IDistanceTracker
    {
        float DistanceMeters { get; }
        event Action<float> OnDistanceChanged;
        void ResetDistance();
        void StartRun();
        void StopRun();
    }
}
