using UnityEngine;
using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Character;
using MagicVillageDash.Player;

namespace MagicVillageDash.Input
{
    [DisallowMultipleComponent]
    public sealed class RunnerSwipeController : SwipeInputSystem
    {
        [Header("Providers")]
        [SerializeField] private MonoBehaviour playerControllerProvider;

        [Header("Options")]
        [Tooltip("If true, a screen tap will trigger Jump() in addition to SwipeUp.")]
        [SerializeField] private bool tapTriggersJump = true;

        //private ISwipeInput swipe;   // depends on abstraction (DIP)
        private IMovementController movementController;  // game motor
        private bool active;
        public bool IsActive => active;
        public bool TapTriggersJump { get => tapTriggersJump; set => tapTriggersJump = value; }

        protected override void Awake()
        {   
            base.Awake();
            movementController = playerControllerProvider as IMovementController ?? FindAnyObjectByType<PlayerController>(FindObjectsInactive.Exclude);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Activate();            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Deactivate();
        }

        
        private void OnSwipeLeft()  => movementController.TurnLeft();
        private void OnSwipeRight() => movementController.TurnRight();
        private void OnSwipeUp()    => movementController.Jump();
        private void OnSwipeDown()  => movementController.Crouch(true);
        private void OnTap()        => movementController.Jump();

        public void Activate()
        {
            if (active) return;
            SwipeLeft+= OnSwipeLeft;
            SwipeRight += OnSwipeRight;
            SwipeUp += OnSwipeUp;
            SwipeDown += OnSwipeDown;
            if (tapTriggersJump) Tap += OnTap;
            active = true;
        }

        public void Deactivate()
        {
            if (!active) return;
            SwipeLeft -= OnSwipeLeft;
            SwipeRight -= OnSwipeRight;
            SwipeUp -= OnSwipeUp;
            SwipeDown -= OnSwipeDown;
            if (tapTriggersJump) Tap -= OnTap;
            active = false;
        }
    }
}
