using System;
using System.Collections;
using ErccDev.Foundation.Camera;
using MagicVillageDash.Camera;
using MagicVillageDash.Character.CharacterAnimator;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ILaneMover))]
    public class EnemyController : MonoBehaviour
    {

        public enum LaneBehavior { Yield, Block, Swap, Ignore }

        [Header("References")]
        [SerializeField] private CameraShaker cameraShakerProvider;
        [SerializeField] private MonoBehaviour playerLaneMoverProvider; 
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CharacterController playerCharacterController;
        [SerializeField] private CharacterAnimatorController selfAnimatorControllerProvider;
        private ICameraShaker cameraShaker;
        private ILaneMover player;
        private ILaneMover self;
        private IMovementAnimator movementAnimator;

        [Header("Behavior")]
        [SerializeField] private LaneBehavior behavior = LaneBehavior.Swap;
        [SerializeField] private float zProximity = 10f;
        [SerializeField] private float reactDelay = 0.06f;

        [Header("Safety")]
        [Tooltip("Failsafe: if something goes wrong, collisions auto-reenable after this many seconds.")]
        [SerializeField] private float collisionsFailSafe = 1.0f;

        private CharacterController selfCharacterController;
        private Coroutine collisionsTimeoutCo;

        /// <summary>Raised when the enemy is death. Factory listens and recycles.</summary>
        public event Action<EnemyController> Ondied;

        void Awake()
        {
            selfCharacterController = GetComponent<CharacterController>();
            self = GetComponent<ILaneMover>();
            movementAnimator = selfAnimatorControllerProvider;
            cameraShaker = cameraShakerProvider;
            player = playerLaneMoverProvider as ILaneMover ?? FindAnyObjectByType<LaneRunner>(FindObjectsInactive.Exclude);                
            if (!playerCharacterController && playerTransform)
                playerCharacterController = playerTransform.GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            if (player == null) return;
            player.OnLaneChangeAttempt += OnPlayerAttempt;            
            self.OnLaneChanged += OnSelfLaneChanged;
        }


        void OnDisable()
        {
            if (player == null) return;
            Ondied?.Invoke(this);
            player.OnLaneChangeAttempt -= OnPlayerAttempt;
            self.OnLaneChanged -= OnSelfLaneChanged;
            SetCollisions(true);
        }


        private void BeginNonCollidingWindow()
        {
            SetCollisions(false);
            if (collisionsTimeoutCo != null) StopCoroutine(collisionsTimeoutCo);
            collisionsTimeoutCo = StartCoroutine(ReenableAfterTimeout(collisionsFailSafe));
        }

        private IEnumerator ReenableAfterTimeout(float t)
        {
            yield return new WaitForSeconds(t);
            SetCollisions(true);
            collisionsTimeoutCo = null;
        }
        
        private void SetCollisions(bool enabled)
        {
            if (!selfCharacterController || !playerCharacterController) return;
            Physics.IgnoreCollision(selfCharacterController, playerCharacterController, !enabled);
        }

        private void OnSelfLaneChanged(int arg1, int arg2)
        {
            SetCollisions(true);
        }

        private void OnPlayerAttempt(int from, int to)
        {
            if (!playerTransform) return;
            if (Mathf.Abs(transform.position.z - playerTransform.position.z) > zProximity) return;
            switch (behavior)
            {
                case LaneBehavior.Yield:
                    if (to != self.CurrentLane) return;
                    BeginNonCollidingWindow();
                    StartCoroutine(DoYieldAfterDelay());
                    break;
                case LaneBehavior.Block:
                    if (to != self.CurrentLane) return;
                    StartCoroutine(DenyAfterDelay(from, to));
                    break;
                case LaneBehavior.Swap:
                    if (to != self.CurrentLane) return;
                    BeginNonCollidingWindow();
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
            if (playerFromLane > self.CurrentLane)
            {
                self.MoveRight();
                movementAnimator.TurnRight();
                cameraShaker.Shake(1.0f, 1.0f, 0.25f);
            }
            else if (playerFromLane < self.CurrentLane)
            {
                self.MoveLeft();
                movementAnimator.TurnLeft();
                cameraShaker.Shake(1.0f, 1.0f, 0.25f);
            }
           
            
            if (reactDelay > 0f)
                yield return new WaitForSeconds(reactDelay);

            player.SnapToLane(playerFromLane);
            self.SnapToLane(toLane);

            // (Optional) feedback here: play "blocked" SFX/VFX/haptic via your systems
        }

        IEnumerator DoMirrorAfterDelay(int targetLane)
        {
            yield return new WaitForSeconds(reactDelay);
            if (targetLane > self.CurrentLane)
            {
                self.MoveRight();
                movementAnimator.TurnRight();
            }
            else if (targetLane < self.CurrentLane)
            {
                self.MoveLeft();    
                movementAnimator.TurnLeft();
            }
        }

        internal void SetSpawnPose(int laneIndex)
        {
            self.SnapToLane(laneIndex);
        }
    }
}
