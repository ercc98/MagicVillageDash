using System;

namespace MagicVillageDash.Runner
{
    /// Minimal lane-capability contract (SRP + ISP).
    public interface ILaneMover
    {
        int  CurrentLane { get; }
        int  LaneCount   { get; }
        float LaneWidth  { get; }

        event Action<int,int> OnLaneChangeAttempt; // (from, to)
        event Action<int,int> OnLaneChanged;       // (from, to)

        void MoveLeft();
        void MoveRight();
        void Jump();
        void Slide();
        void SnapToLane(int lane);
    }
}
