using UnityEngine;
using MagicVillageDash.Score;
using TMPro;

namespace MagicVillageDash.UI
{
    public sealed class CoinHUDText : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CoinCounter coinCounterProvider;
        [SerializeField] private string format = "{0}";

        ICoinCounter coinCounter;

        void Awake()
        {
            if (!label) label = GetComponent<TMP_Text>();
            coinCounter = coinCounterProvider as ICoinCounter ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
            
        }

        void OnEnable()
        {
            coinCounter?.ResetCoins(coinCounter.Coins); // fuerza actualización
            UpdateText(coinCounter != null ? coinCounter.Coins : 0);
            coinCounter.CoinsChanged += UpdateText;
        }

        void OnDisable()
        {
            if (coinCounter == null) return;
            coinCounter.CoinsChanged -= UpdateText;
        }

        public void UpdateText(int total) { if (label) label.text = string.Format(format, coinCounter.Coins); }
    }
}