using UnityEngine;
using UnityEngine.InputSystem;
using ErccDev.Foundation.Input.Pinch;
using ErccDev.Foundation.Input.Touch;

namespace MagicVillageDash.Den.Camera
{
    /// <summary>
    /// Drag-to-pan + pinch-to-zoom for the fixed isometric Den camera. Pan is driven by the
    /// single-finger steering axes (<see cref="IHorizontalTouchInput.SteeringX"/> /
    /// <see cref="IVerticalTouchInput.SteeringY"/>) — the further you drag from the press point,
    /// the faster the camera glides. Zoom is driven by <see cref="IPinchInput"/>: fingers apart =
    /// zoom in, together = zoom out. Both are clamped; pan is suppressed while pinching so the
    /// two gestures don't fight.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DenCameraController : MonoBehaviour
    {
        [Header("Providers (leave empty to auto-find in scene)")]
        [SerializeField] private MonoBehaviour steeringProvider;   // SteeringTouchInputSystem
        [SerializeField] private MonoBehaviour pinchProvider;      // PinchInputSystem
        [SerializeField] private UnityEngine.Camera targetCamera;  // defaults to Camera.main
        [Tooltip("Transform moved when panning. Defaults to the target camera's transform.")]
        [SerializeField] private Transform panTarget;

        [Header("Pan")]
        [Tooltip("World units per second at full steering (steer = 1).")]
        [Min(0f)] [SerializeField] private float panSpeed = 12f;
        [Tooltip("Center of the rectangular pan area, on the XZ plane.")]
        [SerializeField] private Vector2 panCenter = Vector2.zero;
        [Tooltip("Half-size of the pan area (X = world X, Y = world Z).")]
        [SerializeField] private Vector2 panHalfExtents = new Vector2(15f, 15f);
        [Tooltip("Invert drag direction (push the world instead of steering toward it).")]
        [SerializeField] private bool invertPan = false;
        [Tooltip("How quickly pan eases toward the steering input. Higher = snappier, lower = more inertia.")]
        [Min(0f)] [SerializeField] private float panSmooth = 10f;

        [Header("Zoom")]
        [Tooltip("Orthographic size change per DPI-scaled pinch pixel.")]
        [Min(0f)] [SerializeField] private float zoomSpeed = 0.03f;
        [Tooltip("Field-of-view change per pinch pixel, used for perspective cameras.")]
        [Min(0f)] [SerializeField] private float perspectiveZoomSpeed = 0.05f;
        [SerializeField] private float minZoom = 4f;
        [SerializeField] private float maxZoom = 14f;

        [Header("Editor")]
        [Tooltip("Mouse scroll wheel zoom so clamping can be verified without a touch device.")]
        [SerializeField] private bool scrollWheelZoom = true;
        [Tooltip("Zoom change per scroll notch (in zoom units).")]
        [Min(0f)] [SerializeField] private float scrollZoomStep = 1f;

        private IHorizontalTouchInput horizontal;
        private IVerticalTouchInput vertical;
        private IPinchInput pinch;

        private float _currentSteerX;
        private float _currentSteerY;

        private void Awake()
        {
            var steering = steeringProvider != null
                ? steeringProvider
                : FindAnyObjectByType<SteeringTouchInputSystem>(FindObjectsInactive.Exclude);

            horizontal = steering as IHorizontalTouchInput;
            vertical   = steering as IVerticalTouchInput;

            pinch = pinchProvider as IPinchInput
                 ?? FindAnyObjectByType<PinchInputSystem>(FindObjectsInactive.Exclude);

            if (targetCamera == null)
                targetCamera = UnityEngine.Camera.main;
            if (panTarget == null && targetCamera != null)
                panTarget = targetCamera.transform;
        }

        private void OnEnable()
        {
            if (pinch == null) return;
            pinch.PinchedIn  += OnPinchedIn;   // fingers closer  -> zoom out
            pinch.PinchedOut += OnPinchedOut;  // fingers apart   -> zoom in
        }

        private void OnDisable()
        {
            if (pinch == null) return;
            pinch.PinchedIn  -= OnPinchedIn;
            pinch.PinchedOut -= OnPinchedOut;
        }

        private void Update()
        {
            ScrollZoom();

            // Pinch owns the gesture while two fingers are down — pan decays to zero underneath it.
            bool pinching = pinch != null && pinch.IsPinching;
            Pan(Time.unscaledDeltaTime, pinching);
        }

        private void ScrollZoom()
        {
            if (!scrollWheelZoom || Mouse.current == null)
                return;

            float scrollY = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Approximately(scrollY, 0f))
                return;

            // Scroll up -> zoom in (smaller view). One notch = one step, direction only.
            ZoomBy(-Mathf.Sign(scrollY) * scrollZoomStep);
        }

        private void Pan(float dt, bool pinching)
        {
            // Target steering is zero while pinching or when there's no input source — so the
            // smoothed value eases back to rest instead of snapping (mirrors the kart's steer lerp).
            bool hasInput = !pinching && horizontal != null && vertical != null;
            float targetX = hasInput ? horizontal.SteeringX : 0f;
            float targetY = hasInput ? vertical.SteeringY   : 0f;

            float t = Mathf.Clamp01(panSmooth * dt);
            _currentSteerX = Mathf.Lerp(_currentSteerX, targetX, t);
            _currentSteerY = Mathf.Lerp(_currentSteerY, targetY, t);

            if (panTarget == null || targetCamera == null)
                return;

            float steerX = _currentSteerX;
            float steerY = _currentSteerY;
            if (Mathf.Abs(steerX) < 0.0001f && Mathf.Abs(steerY) < 0.0001f)
                return;

            if (invertPan)
            {
                steerX = -steerX;
                steerY = -steerY;
            }

            // Screen-relative pan flattened onto the ground (XZ) plane.
            Vector3 right   = Vector3.ProjectOnPlane(targetCamera.transform.right,   Vector3.up).normalized;
            Vector3 forward = Vector3.ProjectOnPlane(targetCamera.transform.forward, Vector3.up).normalized;

            Vector3 move = (right * steerX + forward * steerY) * (panSpeed * dt);

            Vector3 pos = panTarget.position + move;
            pos.x = Mathf.Clamp(pos.x, panCenter.x - panHalfExtents.x, panCenter.x + panHalfExtents.x);
            pos.z = Mathf.Clamp(pos.z, panCenter.y - panHalfExtents.y, panCenter.y + panHalfExtents.y);
            panTarget.position = pos;
        }

        private void OnPinchedIn()  => ZoomBy(+Mathf.Abs(pinch.DeltaPixels) * ZoomSpeed);  // shrink view = zoom out
        private void OnPinchedOut() => ZoomBy(-Mathf.Abs(pinch.DeltaPixels) * ZoomSpeed);  // grow view   = zoom in

        private float ZoomSpeed => targetCamera != null && targetCamera.orthographic ? zoomSpeed : perspectiveZoomSpeed;

        // delta: signed change in zoom units (+ zoom out / bigger, - zoom in / smaller). Clamped.
        private void ZoomBy(float delta)
        {
            if (targetCamera == null || Mathf.Approximately(delta, 0f))
                return;

            if (targetCamera.orthographic)
                targetCamera.orthographicSize = Mathf.Clamp(targetCamera.orthographicSize + delta, minZoom, maxZoom);
            else
                targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView + delta, minZoom, maxZoom);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 c = new Vector3(panCenter.x, 0f, panCenter.y);
            Vector3 s = new Vector3(panHalfExtents.x * 2f, 0.1f, panHalfExtents.y * 2f);
            Gizmos.DrawWireCube(c, s);
        }
#endif
    }
}
