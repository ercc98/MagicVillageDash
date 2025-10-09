using UnityEngine;
using TMPro;
using MagicVillageDash.Score;

namespace MagicVillageDash.UI
{
    public sealed class ScoreHUDText : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private RunScoreSystem score;
        [SerializeField] private string format = "{0}";

        void Awake()
        {
            if (!label) label = GetComponent<TMP_Text>();
            if (!score) score = FindAnyObjectByType<RunScoreSystem>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            if (score) score.ScoreChanged += OnScoreChanged;
            OnScoreChanged(score ? score.CurrentScore : 0);
        }

        void OnDisable()
        {
            if (score) score.ScoreChanged -= OnScoreChanged;
        }

        void OnScoreChanged(int value)
        {
            if (!label) return;
            label.text = string.Format(format, value);
        }
    }
}