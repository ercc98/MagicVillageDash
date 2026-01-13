using UnityEngine;
using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Character;
using MagicVillageDash.Player;
using ErccDev.Foundation.Core.Gameplay;
using System;
using MagicVillageDash.Runner;

namespace MagicVillageDash.Input
{
    [DisallowMultipleComponent]
    public sealed class RunnerSwipeController : MonoBehaviour, IRunnerInputController 
    {
        [Header("Providers")]
        [SerializeField] private MonoBehaviour playerControllerProvider;
        [SerializeField] private MonoBehaviour swipeInputProvider;

        [Header("Options")]
        [Tooltip("If true, a screen tap will trigger Jump() in addition to SwipeUp.")]
        [SerializeField] private bool tapTriggersJump = true;
        private IMovementController movementController;
        private ISwipeInput swipeInput;
        private bool active;
        public bool IsActive => active;
        public bool TapTriggersJump { get => tapTriggersJump; set => tapTriggersJump = value; }

        private void Awake()
        {   
            swipeInput = swipeInputProvider as ISwipeInput ?? FindAnyObjectByType<SwipeInputSystem>(FindObjectsInactive.Exclude);;
            movementController = playerControllerProvider as IMovementController ?? FindAnyObjectByType<PlayerController>(FindObjectsInactive.Exclude);
        }

        private void OnEnable()
        {
            GameEvents.GameOver += OnGameOver;
            Activate();            
        }

        private void OnDisable()
        {
            GameEvents.GameOver -= OnGameOver;
            Deactivate();
        }

        private void OnGameOver()
        {
            Deactivate();
        }

        private void OnSwipeLeft()  => TurnLeft();
        private void OnSwipeRight() => TurnRight();
        private void OnSwipeUp()    => Jump();
        private void OnSwipeDown()  => Crouch(true);
        private void OnTap()        => Jump();

        public void Activate()
        {
            if (active) return;
            swipeInput.SwipeLeft+= OnSwipeLeft;
            swipeInput.SwipeRight += OnSwipeRight;
            swipeInput.SwipeUp += OnSwipeUp;
            swipeInput.SwipeDown += OnSwipeDown;
            if (tapTriggersJump) swipeInput.Tap += OnTap;
            active = true;
        }

        public void Deactivate()
        {
            if (!active) return;
            swipeInput.SwipeLeft -= OnSwipeLeft;
            swipeInput.SwipeRight -= OnSwipeRight;
            swipeInput.SwipeUp -= OnSwipeUp;
            swipeInput.SwipeDown -= OnSwipeDown;
            if (tapTriggersJump) swipeInput.Tap -= OnTap;
            active = false;
        }

        private void TurnLeft()
        {
            movementController.TurnLeft();
        }

        private void TurnRight()
        {
            movementController.TurnRight();
        }

        private void Jump()
        {
            movementController.Jump();
        }

        private void Crouch(bool isCrouching)
        {
            movementController.Crouch(isCrouching);
        }
    }
}
