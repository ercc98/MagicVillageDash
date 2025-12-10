namespace MagicVillageDash.Score
{
    public interface IRunScoreSystem
    {
        int CurrentScore { get; }
        void ResetRun();
        void CommitIfBest();
    }
}
