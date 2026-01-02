namespace MagicVillageDash.World
{
    /// <summary>
    /// Something that can fill a chunk with content (coins, obstacles, decorations, etc.).
    /// The spawner or orchestrator decides when to call FillChunk.
    /// </summary>
    public interface IChunkFiller
    {
        void FillChunk(ChunkRoot chunk);
    }
}