using System;

namespace MagicVillageDash.Score
{
    public interface IDistanceTracker
    {
        float CurrentDistance { get; }
        void ResetDistance();
        void StartRun();
        void StopRun();
    }
}
