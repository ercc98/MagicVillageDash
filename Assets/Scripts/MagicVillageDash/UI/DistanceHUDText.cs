using UnityEngine;
using TMPro;
using MagicVillageDash.Score;

namespace MagicVillageDash.UI
{
    public sealed class DistanceHUDText : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private DistanceTracker distance;
        [SerializeField] private string format = "{0} m"; // e.g., "123 m"

        void Awake()
        {
            if (!label)    label = GetComponent<TMP_Text>();
            if (!distance) distance = FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            if (distance) distance.DistanceChanged += OnDistanceChanged;
            OnDistanceChanged(distance ? distance.DistanceMeters : 0f);
        }

        void OnDisable()
        {
            if (distance) distance.DistanceChanged -= OnDistanceChanged;
        }

        void OnDistanceChanged(float meters)
        {
            if (!label) return;
            label.text = string.Format(format, Mathf.FloorToInt(meters));
        }
    }
}