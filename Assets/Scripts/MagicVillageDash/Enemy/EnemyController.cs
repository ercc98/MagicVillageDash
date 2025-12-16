using System;
using System.Collections;
using ErccDev.Foundation.Camera;
using MagicVillageDash.Camera;
using MagicVillageDash.Character;
using MagicVillageDash.Character.CharacterAnimator;
using MagicVillageDash.Gameplay;
using MagicVillageDash.Obstacles;
using MagicVillageDash.Runner;
using MagicVillageDash.World;
using Unity.Collections;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyController : MonoBehaviour, IMovementController, IHazardReceiver
    {

        [Header("References")]
        [SerializeField] private ParticleSystem hitparticlesProvider;
        [SerializeField] private MonoBehaviour playerLaneMoverProvider; 
        [SerializeField] private CharacterController playerCharacterController;
        [SerializeField] private CharacterAnimatorController selfAnimatorControllerProvider;
        [SerializeField] private ParticleSystem hitHazardParticlesProvider;
        [SerializeField] private MonoBehaviour gameSpeedProvider;
        
        private ILaneMover player;
        private ILaneMover selfLaneMover;
        private IMovementAnimator movementAnimator;
        IGameSpeedController    gameSpeedController;


        [Header("Safety")]
        [Tooltip("Failsafe: if something goes wrong, collisions auto-reenable after this many seconds.")]
        [SerializeField] private float collisionsFailSafe = 1.0f;

        private CharacterController selfCharacterController;
        private Coroutine collisionsTimeoutCo;

        public ILaneMover SelfLaneMover => selfLaneMover;
        /// <summary>Raised when the enemy is death. Factory listens and recycles.</summary>
        public event Action<EnemyController> Ondied;

        void Awake()
        {
            selfCharacterController = GetComponent<CharacterController>();
            selfLaneMover = GetComponent<ILaneMover>();
            movementAnimator = selfAnimatorControllerProvider;
            player = playerLaneMoverProvider as ILaneMover ?? playerLaneMoverProvider.GetComponent<ILaneMover>();       
            gameSpeedController = gameSpeedProvider as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            if (player == null) return;
            player.OnLaneChangeAttempt += OnPlayerAttempt;            
            selfLaneMover.OnLaneChanged += OnSelfLaneChanged;
        }


        void OnDisable()
        {
            if (player == null) return;
            player.OnLaneChangeAttempt -= OnPlayerAttempt;
            selfLaneMover.OnLaneChanged -= OnSelfLaneChanged;
            SetCollisions(true);
        }

        void Update()
        {
            MovingSpeed(Mathf.Clamp(gameSpeedController.CurrentSpeed * .025f, 0f, 1f));
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
            Physics.ContactEvent += contactEventHandler;
        }

        private void contactEventHandler(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly headerArray)
        {
            throw new NotImplementedException();
        }

        private void OnSelfLaneChanged(int from, int to)
        {
            SetCollisions(true);
        }

        private void OnPlayerAttempt(int from, int to)
        {
            if (to != selfLaneMover.CurrentLane) return;
            BeginNonCollidingWindow();

        }

        internal void SetSpawnPose(int laneIndex)
        {
            selfLaneMover.SnapToLane(laneIndex);
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
            movementAnimator.MovingSpeed(speed);
        }

        public void Crouch(bool isCrouching)
        {
            selfLaneMover.Slide();
            movementAnimator.Crouch(isCrouching);
        }

        public void Jump()
        {
            selfLaneMover.Jump();
            movementAnimator.Jump();
        }
        #endregion

        public void OnHazardHit(Vector3 hazardHitPosition)
        {
            SpawnHitVfx(hazardHitPosition);
            Ondied?.Invoke(this);
            gameObject.SetActive(false);
        }

        private void SpawnHitVfx(Vector3 hitPosition)
        {
            if (!hitHazardParticlesProvider) return;
            hitHazardParticlesProvider.transform.SetPositionAndRotation(hitPosition, Quaternion.identity);
            hitHazardParticlesProvider.Play();
        }

    }
}
