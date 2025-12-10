namespace MagicVillageDash.World
{
    public interface IGameSpeedController
    {
        float CurrentSpeed { get; }
        void SetSpeed(float value);
        void ResetSpeed();
    }
}
