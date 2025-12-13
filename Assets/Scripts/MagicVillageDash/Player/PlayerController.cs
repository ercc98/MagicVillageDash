using System;
using System.Collections;
using ErccDev.Foundation.Input.Swipe;
using MagicVillageDash.Input;
using MagicVillageDash.Runner;
using NUnit.Framework;
using UnityEngine;

namespace MagicVillageDash.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public enum LaneBehavior { Yield, Block, Swap, Ignore }
        [SerializeField] private MonoBehaviour selfLaneMoverProvider;
        [SerializeField] private MonoBehaviour selfRunnerInputProvider;
        [SerializeField] private MonoBehaviour swipeInputProvider;
        [SerializeField] private MonoBehaviour enemyLaneMoverProvider;
        [SerializeField] private Transform enemyTransform; 
        private CharacterController selfCharacterController;
        private ILaneMover selfLaneMover;
        private IRunnerInputController inputController;
        private ISwipeInput swipeInput;
        private ILaneMover enemyLaneMover;

        [Header("Behavior")]
        [SerializeField] private LaneBehavior behavior = LaneBehavior.Swap;
        [SerializeField] private float zProximity = 10f;
        [SerializeField] private float reactDelay = 0.06f;

        void Awake()
        {
            inputController = selfRunnerInputProvider as IRunnerInputController;
            selfLaneMover = selfLaneMoverProvider as ILaneMover;
            swipeInput = swipeInputProvider as ISwipeInput;
            enemyLaneMover = enemyLaneMoverProvider as ILaneMover;

            selfCharacterController = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            inputController.SetMover(selfLaneMover);

            inputController.SetInput(swipeInput);

            inputController.Activate();

            if (enemyLaneMover == null) return;
            enemyLaneMover.OnLaneChangeAttempt += OnEnemyAttempt;            
            selfLaneMover.OnLaneChanged += OnSelfLaneChanged;
        }



        void OnDisable()
        {
            if (enemyLaneMover == null) return;
            enemyLaneMover.OnLaneChangeAttempt -= OnEnemyAttempt;
            selfLaneMover.OnLaneChanged -= OnSelfLaneChanged;
        }
        
        private void OnSelfLaneChanged(int arg1, int arg2)
        {
            
        }

        private void OnEnemyAttempt(int from, int to)
        {
            if (!enemyTransform) return;
            if (Mathf.Abs(transform.position.z - enemyTransform.position.z) > zProximity) return;
            switch (behavior)
            {
                case LaneBehavior.Yield:
                    if (to != selfLaneMover.CurrentLane) return;
                    StartCoroutine(DoYieldAfterDelay());
                    break;
                case LaneBehavior.Block:
                    if (to != selfLaneMover.CurrentLane) return;
                    StartCoroutine(DenyAfterDelay(from, to));
                    break;
                case LaneBehavior.Swap:
                    if (to != selfLaneMover.CurrentLane) return;
                    StartCoroutine(DoMirrorAfterDelay(from));
                    break;

                case LaneBehavior.Ignore:
                default:
                    break;
            }
        }

        IEnumerator DoYieldAfterDelay()
        {
            throw new NotImplementedException();
        }
        
        private IEnumerator DenyAfterDelay(int playerFromLane, int toLane)
        {
            if (playerFromLane > selfLaneMover.CurrentLane) selfLaneMover.MoveRight();
            else if (playerFromLane < selfLaneMover.CurrentLane) selfLaneMover.MoveLeft();
            if (reactDelay > 0f)
                yield return new WaitForSeconds(reactDelay);

            enemyLaneMover.SnapToLane(playerFromLane);
            selfLaneMover.SnapToLane(toLane);

            // (Optional) feedback here: play "blocked" SFX/VFX/haptic via your systems
        }

        IEnumerator DoMirrorAfterDelay(int targetLane)
        {
            yield return new WaitForSeconds(reactDelay);
            if (targetLane > selfLaneMover.CurrentLane) selfLaneMover.MoveRight();
            else if (targetLane < selfLaneMover.CurrentLane) selfLaneMover.MoveLeft();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
