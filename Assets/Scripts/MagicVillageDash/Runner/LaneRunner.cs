using System;
using UnityEngine;

namespace MagicVillageDash.Runner
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class LaneRunner : MonoBehaviour, ILaneMover
    {
        [Header("Lanes")]
        [SerializeField] private int   laneCount = 3;
        [SerializeField] private float laneWidth = 2.2f;
        [Tooltip("Max horizontal speed when shifting between lanes (units/sec).")]
        [SerializeField] private float laneSpeed = 12f;
        [Tooltip("How close to lane center to consider the change finished.")]
        [SerializeField, Range(0.001f, 0.2f)] private float laneSnapEpsilon = 0.02f;

        [Header("Jump / Gravity")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity   = -30f;

        [Header("Slide")]
        [SerializeField] private float slideDuration = 0.6f;
        [SerializeField] private int initialLane = 1;

        private CharacterController characterController;

        private int   currentLane;   // 0 .. laneCount-1
        private float targetX;       // world-space X for the current lane
        private float verticalVel;   // Y velocity (m/s)

        private bool  sliding;
        private float slideTimer;

        // Save original collider to cleanly restore after slide
        private float   originalHeight;
        private Vector3 originalCenter;

        public event Action<int, int> OnLaneChangeAttempt;
        public event Action<int, int> OnLaneChanged;

        public int CurrentLane => currentLane;

        public int LaneCount => laneCount;

        public float LaneWidth => laneWidth;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();

            // Begin centered lane
            currentLane = initialLane;
            targetX     = LaneToX(currentLane);

            // Cache original collider settings
            originalHeight = characterController.height;
            originalCenter = characterController.center;

            // Optional: ensure we start slightly above ground (for isGrounded to settle)
            var p = transform.position;
            var minY = originalHeight * 0.5f + 0.01f;
            if (p.y < minY) transform.position = new Vector3(p.x, minY, p.z);

            // Recommended in Inspector:
            // CharacterController.Min Move Distance = 0.0001 for precise grounding
        }

        void Update()
        {
            float dt = Time.deltaTime;

            // --- Horizontal: use CharacterController.Move exclusively ---
            // Compute the desired per-frame horizontal delta toward targetX, clamped by laneSpeed.
            float dxMax = laneSpeed * dt;
            float dx    = Mathf.Clamp(targetX - transform.position.x, -dxMax, dxMax);

            // --- Vertical: gravity & jump ---
            if (characterController.isGrounded && verticalVel < 0f)
                verticalVel = -2f; // small downward bias keeps contact with ground

            verticalVel += gravity * dt;
            float dy = verticalVel * dt;

            // Compose delta (no forward Z: world moves toward player)
            characterController.Move(new Vector3(dx, dy, 0f));

            // --- Slide timer ---
            if (sliding)
            {
                slideTimer -= dt;
                if (slideTimer <= 0f) EndSlide();
            }

            // Detect arrival to target lane center and emit Changed once
            if (Mathf.Abs(transform.position.x - targetX) <= laneSnapEpsilon)
            {
                // snap for stability
                var pos = transform.position; pos.x = targetX; transform.position = pos;

                int newLane = XToLane(targetX);
                if (newLane != currentLane)
                {
                    int prev = currentLane;
                    currentLane = newLane;
                    OnLaneChanged?.Invoke(prev, currentLane);
                }
            }
        }

        // ===== Public API (called by input adapter) =====

        public void MoveLeft()
        {
            if (currentLane <= 0) return;
            RequestLane(currentLane - 1);
        }

        public void MoveRight()
        {
            if (currentLane >= laneCount - 1) return;
            RequestLane(currentLane + 1);
        }

        public void Jump()
        {
            if (characterController.isGrounded && !sliding)
                verticalVel = jumpForce;
        }

        public void Slide()
        {
            if (!sliding && characterController.isGrounded)
            {
                sliding   = true;
                slideTimer = slideDuration;

                // Reduce collider height safely
                characterController.height = originalHeight * 0.5f;
                characterController.center = new Vector3(originalCenter.x, characterController.height * 0.5f, originalCenter.z);
            }
        }

        // ===== Helpers =====

        private void EndSlide()
        {
            sliding = false;
            characterController.height = originalHeight;
            characterController.center = originalCenter;
        }

        private void RequestLane(int toLane)
        {
            toLane = Mathf.Clamp(toLane, 0, laneCount - 1);
            OnLaneChangeAttempt?.Invoke(currentLane, toLane); // announce intent
            targetX = LaneToX(toLane);                        // start sliding; 'Changed' fires on arrival
        }

        private float LaneToX(int laneIndex)
        {
            // Center lane around X=0
            return (laneIndex - (laneCount - 1) * 0.5f) * laneWidth;
        }

        public void SnapToLane(int lane)
        {
            lane = Mathf.Clamp(lane, 0, laneCount - 1);
            currentLane = lane;
            targetX     = LaneToX(lane);
            var p = transform.position; p.x = targetX; transform.position = p;
        }

        private int XToLane(float xTarget)
        {
            float idxF = xTarget / laneWidth + (laneCount - 1) * 0.5f;
            return Mathf.Clamp(Mathf.RoundToInt(idxF), 0, laneCount - 1);
        }
    }
}
