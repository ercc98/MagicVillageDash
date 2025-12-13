using System;
using System.Collections;
using MagicVillageDash.Character;
using MagicVillageDash.Character.CharacterAnimator;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(LaneRunner))]
    [RequireComponent(typeof(MonoBehaviour))]
    [RequireComponent(typeof(ILaneMover))]
    public class PlayerController : MonoBehaviour, IMovementController
    {
        public enum LaneBehavior { Yield, Block, Swap, Ignore }
        [SerializeField] private CharacterAnimatorController selfAnimatorControllerProvider;
        [SerializeField] private Transform enemyTransform; 
        private ILaneMover selfLaneMover;
        private ILaneMover enemyLaneMover;
        private IMovementAnimator movementAnimator;

        [Header("Behavior")]
        [SerializeField] private LaneBehavior behavior = LaneBehavior.Swap;
        [SerializeField] private float zProximity = 10f;
        [SerializeField] private float reactDelay = 0.06f;

        void Awake()
        {
            selfLaneMover = GetComponent<ILaneMover>();
            enemyLaneMover = enemyTransform as ILaneMover ?? GetComponent<MonoBehaviour>() as ILaneMover;
            movementAnimator = selfAnimatorControllerProvider;
        }

        void OnEnable()
        {
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
            if (targetLane > selfLaneMover.CurrentLane)
            {
                selfLaneMover.MoveRight();
                movementAnimator.TurnRight();
            }
            else if (targetLane < selfLaneMover.CurrentLane)
            {
                selfLaneMover.MoveLeft();    
                movementAnimator.TurnLeft();
            }
            
        }

        #region IMovementController Implementation
        public void TurnLeft()
        {
            selfLaneMover.MoveLeft();
            movementAnimator.TurnLeft();
            
        }

        public void TurnRight()
        {
            selfLaneMover.MoveRight();
            movementAnimator.TurnRight();
        }

        public void MovingSpeed(float speed)
        {
            throw new NotImplementedException();
        }

        public void Crouch(bool isCrouching)
        {
            throw new NotImplementedException();
        }

        public void Jump()
        {
            selfLaneMover.Jump();
            movementAnimator.Jump();
        }

        public void Landing()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
