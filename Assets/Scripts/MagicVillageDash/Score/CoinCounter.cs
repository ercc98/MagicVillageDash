using System;
using UnityEngine;
using UnityEngine.Events;

namespace MagicVillageDash.Score
{
    public sealed class CoinCounter : MonoBehaviour, ICoinCounter
    {
        public int Coins { get; private set; }
        // Code subscribers (typed, safe)
        public event Action<int> CoinsChanged;

        public void Add(int amount)
        {
            if (amount <= 0) return;
            Coins += amount;
            CoinsChanged?.Invoke(Coins);
        }

        public void ResetCoins(int value = 0)
        {
            Coins = value;
            CoinsChanged?.Invoke(Coins);
        }
    }
}
