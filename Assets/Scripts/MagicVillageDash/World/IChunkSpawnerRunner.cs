namespace MagicVillageDash.World
{
    public interface IChunkSpawnerRunner
    {
        void StartSpawning();
        void StopSpawning();
        bool IsSpawning { get; }
    }
}