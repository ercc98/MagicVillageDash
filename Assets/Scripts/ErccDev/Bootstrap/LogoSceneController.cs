
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

namespace ErccDev.Bootstrap
{
    /// <summary>
    /// Generic logo scene controller (New Input System):
    /// - Plays a VideoPlayer clip or URL.
    /// - Optional skip via an InputAction (Space, Start, Touch, Mouse by default).
    /// - On finish/error/skip, loads the next scene.
    /// - Uses ErccDev.Loader.SceneLoader if present, else SceneManager.
    /// </summary>
    [AddComponentMenu("ErccDev/Bootstrap/Logo Scene Controller")]
    [RequireComponent(typeof(VideoPlayer))]
    public sealed class LogoSceneController : MonoBehaviour
    {
        [Header("Next Scene")]
        [SerializeField] private string nextSceneName = "Game";
        [Tooltip("If true and a SceneLoader exists in scene, use it to load the next scene.")]
        [SerializeField] private bool preferSceneLoader = true;

        [Header("Playback")]
        [Tooltip("Wait for the video to prepare before playing (recommended).")]
        [SerializeField] private bool waitForPrepare = true;
        [Tooltip("Wait until the first frame is displayed (avoids a black flash on some platforms).")]
        [SerializeField] private bool waitForFirstFrame = true;
        [Tooltip("Guarantee the logo stays on screen at least this long (seconds).")]
        [SerializeField, Min(0f)] private float minDisplaySeconds = 0f;

        [Header("Skip (New Input System)")]
        [Tooltip("Optional input action used to skip the logo (performed triggers Skip). "
               + "If left empty, a default action is created: Space / Gamepad Start / Mouse Left / Touch Press.")]
        [SerializeField] private InputActionProperty skipAction;

        private VideoPlayer _vp;
        private bool _finished;
        private float _startTime;

        // if we create a default action, we own/disable/dispose it
        private bool _ownsAction;

        void Awake()
        {
            _vp = GetComponent<VideoPlayer>();
            _vp.isLooping   = false;
            _vp.playOnAwake = false;
            _vp.skipOnDrop  = true;

            _vp.loopPointReached += OnVideoFinished;
            _vp.errorReceived     += OnVideoError;

            EnsureSkipAction();
        }

        IEnumerator Start()
        {
            bool hasVideo = _vp.clip != null || !string.IsNullOrEmpty(_vp.url);
            _startTime = Time.unscaledTime;

            if (!hasVideo)
            {
                if (minDisplaySeconds > 0f)
                    yield return new WaitForSecondsRealtime(minDisplaySeconds);
                Finish();
                yield break;
            }

            if (waitForPrepare)
            {
                _vp.Prepare();
                while (!_vp.isPrepared) yield return null;
            }

            _vp.Play();

            if (waitForFirstFrame)
            {
                while (_vp.isPlaying && _vp.frame <= 0) yield return null;
            }
        }

        void OnEnable()
        {
            if (skipAction.action != null)
            {
                skipAction.action.performed += OnSkipPerformed;
                if (!skipAction.action.enabled) skipAction.action.Enable();
            }
        }

        void OnDisable()
        {
            if (skipAction.action != null)
            {
                skipAction.action.performed -= OnSkipPerformed;
                if (_ownsAction) skipAction.action.Disable();
            }
        }

        void OnDestroy()
        {
            _vp.loopPointReached -= OnVideoFinished;
            _vp.errorReceived     -= OnVideoError;

            if (_ownsAction && skipAction.action != null)
            {
                skipAction.action.Dispose();
            }
        }

        // Public API so you can skip from UI or other systems if you want
        public void Skip() => Finish();

        // ----- Input -----
        void EnsureSkipAction()
        {
            if (skipAction.action != null) return;

            // Build a default action with sensible bindings
            var a = new InputAction("Skip", InputActionType.Button);
            a.AddBinding("<Keyboard>/space");
            a.AddBinding("<Gamepad>/start");
            a.AddBinding("<Mouse>/leftButton");
            a.AddBinding("<Touchscreen>/primaryTouch/press");
            skipAction = new InputActionProperty(a);
            _ownsAction = true;
        }

        void OnSkipPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) Skip();
        }

        // ----- Video callbacks -----
        void OnVideoFinished(VideoPlayer _) => Finish();

        void OnVideoError(VideoPlayer _, string msg)
        {
            Debug.LogWarning($"[LogoScene] Video error: {msg}");
            Finish();
        }

        // ----- Load next scene -----
        void Finish()
        {
            if (_finished) return;
            _finished = true;

            float shown = Time.unscaledTime - _startTime;
            float remain = Mathf.Max(0f, minDisplaySeconds - shown);
            if (remain > 0f)
            {
                StartCoroutine(LoadAfterDelay(remain));
            }
            else
            {
                LoadNow();
            }
        }

        IEnumerator LoadAfterDelay(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            LoadNow();
        }

        void LoadNow()
        {
            if (preferSceneLoader)
            {
                var loader = FindAnyObjectByType<Loader.SceneLoader>(FindObjectsInactive.Exclude);
                if (loader != null)
                {
                    loader.LoadSceneAsync(nextSceneName, additive: false);
                    return;
                }
            }
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}
