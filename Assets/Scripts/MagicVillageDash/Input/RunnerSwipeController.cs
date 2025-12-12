// Assets/Scripts/MagicVillageDash/Input/RunnerSwipeController.cs
// Glue: subscribes to ErccDev.Input.ISwipeInput and commands MagicVillageDash.Runner.LaneRunner.

using UnityEngine;
using MagicVillageDash.Runner;
using ErccDev.Foundation.Input.Swipe;

namespace MagicVillageDash.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LaneRunner))]
    public sealed class RunnerSwipeController : MonoBehaviour
    {
        [Header("Input Provider")]
        [Tooltip("Assign a component that implements ISwipeInput (e.g., ErccDev.Foundation.Input.SwipeInputSystem).")]
        [SerializeField] private MonoBehaviour swipeProvider;
        [SerializeField] private MonoBehaviour laneRunnerProvider;

        [Header("Options")]
        [Tooltip("If true, a screen tap will trigger Jump() in addition to SwipeUp.")]
        [SerializeField] private bool tapTriggersJump = true;

        private ISwipeInput swipe;   // depends on abstraction (DIP)
        private ILaneMover  runner;  // game motor

        void Awake()
        {
            runner = laneRunnerProvider as ILaneMover ?? FindAnyObjectByType<LaneRunner>(FindObjectsInactive.Exclude);
            swipe = swipeProvider as ISwipeInput ?? FindAnyObjectByType<SwipeInputSystem>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            if (swipe == null) return;

            swipe.SwipeLeft  += OnSwipeLeft;
            swipe.SwipeRight += OnSwipeRight;
            swipe.SwipeUp    += OnSwipeUp;
            swipe.SwipeDown  += OnSwipeDown;

            if (tapTriggersJump)
                swipe.Tap += OnTap;
        }

        void OnDisable()
        {
            if (swipe == null) return;

            swipe.SwipeLeft  -= OnSwipeLeft;
            swipe.SwipeRight -= OnSwipeRight;
            swipe.SwipeUp    -= OnSwipeUp;
            swipe.SwipeDown  -= OnSwipeDown;

            if (tapTriggersJump)
                swipe.Tap -= OnTap;
        }

        // Input handlers → thin delegates to runner motor
        private void OnSwipeLeft()  => runner.MoveLeft();
        private void OnSwipeRight() => runner.MoveRight();
        private void OnSwipeUp()    => runner.Jump();
        private void OnSwipeDown()  => runner.Slide();
        private void OnTap()        => runner.Jump();

#if UNITY_EDITOR
        void OnValidate()
        {
            if (swipeProvider != null && swipeProvider is not ISwipeInput)
            {
                Debug.LogWarning(
                    $"[RunnerSwipeController] '{swipeProvider.GetType().Name}' doesn't implement {nameof(ISwipeInput)}.",
                    this);
            }
        }
#endif
    }
}
