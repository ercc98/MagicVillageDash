using System;
using System.Collections;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyController : MonoBehaviour
    {

        public enum LaneBehavior { Yield, Block, Swap, Ignore }

        [Header("References")]
        [SerializeField] private MonoBehaviour playerLaneMoverProvider; 
        [SerializeField] private MonoBehaviour selfLaneMoverProvider; 
        [SerializeField] private Transform playerTransform; 
        [SerializeField] private CharacterController playerCharacterController;        
        private ILaneMover player;
        private ILaneMover self;

        [Header("Behavior")]
        [SerializeField] private LaneBehavior behavior = LaneBehavior.Swap;
        [SerializeField] private float zProximity = 10f;
        [SerializeField] private float reactDelay = 0.06f;

        [Header("Safety")]
        [Tooltip("Failsafe: if something goes wrong, collisions auto-reenable after this many seconds.")]
        [SerializeField] private float collisionsFailSafe = 1.0f;

        private CharacterController characterController;
        private Coroutine collisionsTimeoutCo;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            player = playerLaneMoverProvider as ILaneMover ?? FindAnyObjectByType<LaneRunner>(FindObjectsInactive.Exclude);            
            self = selfLaneMoverProvider as ILaneMover ?? FindAnyObjectByType<LaneRunner>(FindObjectsInactive.Exclude);            

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
            if (!characterController || !playerCharacterController) return;
            Physics.IgnoreCollision(characterController, playerCharacterController, !enabled);
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
                    StartCoroutine(DenyAfterDelay(from));
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
        
        private IEnumerator DenyAfterDelay(int playerFromLane)
        {
            if (reactDelay > 0f)
                yield return new WaitForSeconds(reactDelay);

            // Snap the player back to their previous lane (cancel the attempt)
            player.SnapToLane(playerFromLane);

            // (Optional) feedback here: play "blocked" SFX/VFX/haptic via your systems
        }

        IEnumerator DoMirrorAfterDelay(int targetLane)
        {
            yield return new WaitForSeconds(reactDelay);
            if (targetLane > self.CurrentLane) self.MoveRight();
            else if (targetLane < self.CurrentLane) self.MoveLeft();
        }
    }
}
