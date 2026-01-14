using UnityEngine;
using ErccDev.Foundation.Pause;

namespace MagicVillageDash.Pause
{
    public sealed class PauseApplier : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour pauseServiceProvider; // IPauseService

        [Header("Things to disable on pause")]
        [SerializeField] private Behaviour[] disableOnPause; // e.g. RunnerInputController, WorldMover, EnemySpawner, etc.

        private IPauseService pause;

        private void Awake()
        {
            pause = pauseServiceProvider as IPauseService;
            if (pause == null)
            {
                Debug.LogError($"{nameof(PauseApplier)}: pauseServiceProvider must implement IPauseService.");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            pause.Changed += OnPauseChanged;
            Apply(pause.IsPaused);
        }

        private void OnDisable()
        {
            if (pause != null) pause.Changed -= OnPauseChanged;
        }

        private void OnPauseChanged(bool isPaused, string reason)
        {
            Apply(isPaused);
        }

        private void Apply(bool isPaused)
        {
            if (disableOnPause == null) return;

            for (int i = 0; i < disableOnPause.Length; i++)
            {
                var b = disableOnPause[i];
                if (b == null) continue;

                // When paused -> disable. When resumed -> enable.
                b.enabled = !isPaused;
            }
        }
    }
}
