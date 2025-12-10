using System;

namespace MagicVillageDash.Score
{
    public interface IDistanceTracker
    {
        float DistanceMeters { get; }
        event Action<float> DistanceChanged;
        void ResetDistance();
    }
}
