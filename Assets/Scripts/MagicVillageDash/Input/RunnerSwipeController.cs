// Assets/Scripts/MagicVillageDash/Input/RunnerSwipeController.cs
// Glue: subscribes to ErccDev.Input.ISwipeInput and commands MagicVillageDash.Runner.LaneRunner.

using UnityEngine;
using ErccDev.Input;              // ISwipeInput (generic lib)
using MagicVillageDash.Runner;    // LaneRunner (your player motor)

namespace MagicVillageDash.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LaneRunner))]
    public sealed class RunnerSwipeController : MonoBehaviour
    {
        [Header("Input Provider")]
        [Tooltip("Assign a component that implements ISwipeInput (e.g., ErccDev.Input.SwipeInputSystem).")]
        [SerializeField] private MonoBehaviour swipeProvider;

        [Header("Options")]
        [Tooltip("If true, a screen tap will trigger Jump() in addition to SwipeUp.")]
        [SerializeField] private bool tapTriggersJump = true;

        [Tooltip("If no provider is assigned, try to auto-find any ISwipeInput in the scene.")]
        [SerializeField] private bool autoFindProvider = true;

        private ISwipeInput swipe;   // depends on abstraction (DIP)
        private LaneRunner  runner;  // game motor

        void Awake()
        {
            runner = GetComponent<LaneRunner>();
            BindProvider(swipeProvider);

            if (swipe == null && autoFindProvider)
            {
                // Try to locate any ISwipeInput in the scene (active or inactive)
                var all = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var mb in all)
                {
                    if (mb is ISwipeInput candidate)
                    {
                        swipeProvider = mb;
                        BindProvider(mb);
                        break;
                    }
                }
            }
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

        private void BindProvider(MonoBehaviour provider)
        {
            swipe = provider as ISwipeInput;
#if UNITY_EDITOR
            if (provider != null && swipe == null)
            {
                Debug.LogWarning(
                    $"[RunnerSwipeController] Assigned provider '{provider.GetType().Name}' " +
                    $"does not implement {nameof(ISwipeInput)}.", this);
            }
            if (provider == null)
            {
                Debug.LogWarning("[RunnerSwipeController] No swipe provider assigned.", this);
            }
#endif
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
