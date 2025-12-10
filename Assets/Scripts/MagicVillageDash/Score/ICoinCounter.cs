using System;

namespace MagicVillageDash.Score
{
    public interface ICoinCounter
    {
        int Coins { get; }
        event Action<int> CoinsChanged;
        void Add(int amount);
        void ResetCoins(int value = 0);
    }
}
