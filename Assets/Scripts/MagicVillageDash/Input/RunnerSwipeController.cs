// Assets/Scripts/MagicVillageDash/Input/RunnerSwipeController.cs
// Glue: subscribes to ErccDev.Input.ISwipeInput and commands MagicVillageDash.Runner.LaneRunner.

using UnityEngine;
using MagicVillageDash.Runner;
using ErccDev.Foundation.Input.Swipe;

namespace MagicVillageDash.Input
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LaneRunner))]
    public sealed class RunnerSwipeController : MonoBehaviour, IRunnerInputController
    {
        [Header("Input Provider")]
        [Tooltip("Assign a component that implements ISwipeInput (e.g., ErccDev.Foundation.Input.SwipeInputSystem).")]
        [SerializeField] private MonoBehaviour swipeProvider;
        [SerializeField] private MonoBehaviour laneRunnerProvider;

        [Header("Options")]
        [Tooltip("If true, a screen tap will trigger Jump() in addition to SwipeUp.")]
        [SerializeField] private bool tapTriggersJump = true;

        private ISwipeInput swipe;   // depends on abstraction (DIP)
        private ILaneMover runner;  // game motor
        private bool active;

        public bool TapTriggersJump { get => tapTriggersJump; set => tapTriggersJump = value; }

        public bool IsActive => active;

        void Awake()
        {
            runner = laneRunnerProvider as ILaneMover ?? FindAnyObjectByType<LaneRunner>(FindObjectsInactive.Exclude);
            swipe = swipeProvider as ISwipeInput ?? FindAnyObjectByType<SwipeInputSystem>(FindObjectsInactive.Exclude);
        }

        void OnEnable()  => Activate();
        void OnDisable() => Deactivate();

        // Input handlers → thin delegates to runner motor
        private void OnSwipeLeft()  => runner.MoveLeft();
        private void OnSwipeRight() => runner.MoveRight();
        private void OnSwipeUp()    => runner.Jump();
        private void OnSwipeDown()  => runner.Slide();
        private void OnTap()        => runner.Jump();

        public void SetInput(ISwipeInput input)  => BindInput(input);
        public void SetMover(ILaneMover m) => runner = m;
        
        private void BindInput(ISwipeInput input)
        {
            if (active) Deactivate();
            swipe = input;
        }

        public void Activate()
        {
            if (active || swipe == null) return;
            swipe.SwipeLeft  += OnSwipeLeft;
            swipe.SwipeRight += OnSwipeRight;
            swipe.SwipeUp    += OnSwipeUp;
            swipe.SwipeDown  += OnSwipeDown;
            if (tapTriggersJump) swipe.Tap += OnTap;
            active = true;
        }

        public void Deactivate()
        {
            if (!active || swipe == null) return;
            swipe.SwipeLeft  -= OnSwipeLeft;
            swipe.SwipeRight -= OnSwipeRight;
            swipe.SwipeUp    -= OnSwipeUp;
            swipe.SwipeDown  -= OnSwipeDown;
            if (tapTriggersJump) swipe.Tap -= OnTap;
            active = false;
        }
    }
}
