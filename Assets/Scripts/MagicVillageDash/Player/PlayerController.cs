using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Character;
using MagicVillageDash.Character.CharacterAnimator;
using MagicVillageDash.Gameplay;
using MagicVillageDash.Runner;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Player
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour, IMovementController, IHazardReceiver
    {
        [Header("References")]
        [SerializeField] private CharacterAnimatorController selfAnimatorControllerProvider;
        [SerializeField] private ParticleSystem hitHazardParticlesProvider;
        [SerializeField] private ParticleSystem TurnRightParticlesProvider;
        [SerializeField] private ParticleSystem TurnLeftParticlesProvider;
        [SerializeField] private MonoBehaviour gameSpeedProvider;
        
        ILaneMover              selfLaneMover;
        IMovementAnimator       movementAnimator;
        IGameSpeedController    gameSpeedController;



        void Awake()
        {
            // Self mover: prefer explicit provider, else GetComponent (KISS).
            selfLaneMover = GetComponent<ILaneMover>();
            movementAnimator = selfAnimatorControllerProvider;
            gameSpeedController = gameSpeedProvider as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
            // Fail-fast validation (small project friendly)
            if (selfLaneMover == null) Debug.LogError("PlayerController: Missing ILaneMover on player.", this);
            if (movementAnimator == null) Debug.LogError("PlayerController: Missing movement animator provider.", this);

        }

        void Update()
        {
            MovingSpeed(Mathf.Clamp(gameSpeedController.CurrentSpeed * .025f, 0f, 1f));
        }

        #region IMovementController Implementation
        public void TurnLeft()
        {
            selfLaneMover.MoveLeft();
            movementAnimator.TurnLeft();
            TurnLeftParticlesProvider.Play();
            
        }

        public void TurnRight()
        {
            selfLaneMover.MoveRight();
            movementAnimator.TurnRight();
            TurnRightParticlesProvider.Play();
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

        public void OnHazardHit(Vector3 hazardHitPosition)
        {
            SpawnHitVfx(hazardHitPosition);
            GameEvents.RaiseGameOver();
        }
        

        #endregion

        private void SpawnHitVfx(Vector3 hitPosition)
        {
            if (!hitHazardParticlesProvider) return;
            hitHazardParticlesProvider.transform.SetPositionAndRotation(hitPosition, Quaternion.identity);
            hitHazardParticlesProvider.Play();
        }
    }
}
