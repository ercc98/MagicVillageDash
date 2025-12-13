using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Runner; // ILaneMover

namespace MagicVillageDash.Input
{
    /// Abstraction for any "input → lane mover" glue (swipe, buttons, gamepad).
    public interface IRunnerInputController
    {
        // Wiring
        void SetInput(ISwipeInput input);
        void SetMover(ILaneMover mover);

        // Options
        bool TapTriggersJump { get; set; }

        // Lifecycle
        void Activate();   // start listening to input
        void Deactivate(); // stop listening

        // State
        bool IsActive { get; }
    }
}