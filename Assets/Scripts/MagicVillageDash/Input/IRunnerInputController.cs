namespace MagicVillageDash.Input
{
    public interface IRunnerInputController
    {

        bool TapTriggersJump { get; set; }

        void Activate();   
        void Deactivate(); 

        bool IsActive { get; }
    }
}