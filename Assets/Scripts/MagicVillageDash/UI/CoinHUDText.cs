using UnityEngine;
using MagicVillageDash.Score;
using UnityEngine.Events;
using TMPro;

namespace MagicVillageDash.UI
{
    public sealed class CoinHUDText : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CoinCounter counter;
        [SerializeField] private string format = "$ {0}";

        void Awake()
        {
            if (!label) label = GetComponent<TMP_Text>();
            if (!counter) counter = FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            if (counter) counter.ResetCoins(counter.Coins); // fuerza actualización
            UpdateText(counter ? counter.Coins : 0);
            if (counter) counter.onCoinsChanged.AddListener(UpdateText);
        }

        void OnDisable()
        {
            if (!counter) return;
            counter.onCoinsChanged.RemoveListener(UpdateText);
        }

        public void UpdateText(int total) { if (label) label.text = string.Format(format, counter.Coins); }
    }
}