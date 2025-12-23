namespace MagicVillageDash.Score
{
    public interface IRunScoreSystem
    {
        int CurrentScore { get; }
        int BestCoins { get; }
        int BestScore { get; }
        float BestDistance { get; }
        void ResetRun();
        void CommitIfBest();
    }
}
