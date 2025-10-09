using UnityEngine;

namespace MagicVillageDash.Runner
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class LaneRunner : MonoBehaviour
    {
        [Header("Lanes")]
        [SerializeField] private int   laneCount = 3;
        [SerializeField] private float laneWidth = 2.2f;
        [Tooltip("Max horizontal speed when shifting between lanes (units/sec).")]
        [SerializeField] private float laneSpeed = 12f;

        [Header("Jump / Gravity")]
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity   = -30f;

        [Header("Slide")]
        [SerializeField] private float slideDuration = 0.6f;

        private CharacterController cc;

        private int   currentLane;   // 0 .. laneCount-1
        private float targetX;       // world-space X for the current lane
        private float verticalVel;   // Y velocity (m/s)

        private bool  sliding;
        private float slideTimer;

        // Save original collider to cleanly restore after slide
        private float   originalHeight;
        private Vector3 originalCenter;

        void Awake()
        {
            cc = GetComponent<CharacterController>();

            // Begin centered lane
            currentLane = laneCount / 2;
            targetX     = LaneToX(currentLane);

            // Cache original collider settings
            originalHeight = cc.height;
            originalCenter = cc.center;

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
            if (cc.isGrounded && verticalVel < 0f)
                verticalVel = -2f; // small downward bias keeps contact with ground

            verticalVel += gravity * dt;
            float dy = verticalVel * dt;

            // Compose delta (no forward Z: world moves toward player)
            Vector3 delta = new Vector3(dx, dy, 0f);
            cc.Move(delta);

            // --- Slide timer ---
            if (sliding)
            {
                slideTimer -= dt;
                if (slideTimer <= 0f) EndSlide();
            }
        }

        // ===== Public API (called by input adapter) =====

        public void MoveLeft()
        {
            if (currentLane > 0)
            {
                currentLane--;
                targetX = LaneToX(currentLane);
            }
        }

        public void MoveRight()
        {
            if (currentLane < laneCount - 1)
            {
                currentLane++;
                targetX = LaneToX(currentLane);
            }
        }

        public void Jump()
        {
            if (cc.isGrounded && !sliding)
                verticalVel = jumpForce;
        }

        public void Slide()
        {
            if (!sliding && cc.isGrounded)
            {
                sliding   = true;
                slideTimer = slideDuration;

                // Reduce collider height safely
                cc.height = originalHeight * 0.5f;
                cc.center = new Vector3(originalCenter.x, cc.height * 0.5f, originalCenter.z);
            }
        }

        // ===== Helpers =====

        private void EndSlide()
        {
            sliding = false;
            cc.height = originalHeight;
            cc.center = originalCenter;
        }

        private float LaneToX(int laneIndex)
        {
            // Center lane around X=0
            return (laneIndex - (laneCount - 1) * 0.5f) * laneWidth;
        }
    }
}
