using UnityEngine;
using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Character;
using MagicVillageDash.Player;
using ErccDev.Foundation.Core.Gameplay;
using System;

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
        private IMovementController movementController;
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
            GameEvents.GameOver += OnGameOver;
            Activate();            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.GameOver -= OnGameOver;
            Deactivate();
        }

        private void OnGameOver()
        {
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
