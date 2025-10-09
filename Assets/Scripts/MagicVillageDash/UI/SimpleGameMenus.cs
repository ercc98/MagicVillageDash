using UnityEngine;
using UnityEngine.SceneManagement;
using ErccDev.Core.Gameplay;
using MagicVillageDash.Score;
using MagicVillageDash.World;

namespace MagicVillageDash.UI
{
    public sealed class SimpleGameMenus : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Optional refs (auto-find if empty)")]
        [SerializeField] private DistanceTracker distance;
        [SerializeField] private CoinCounter     coins;
        [SerializeField] private RunScoreSystem  score;
        [SerializeField] private GameSpeedController speed;

        bool _started;
        bool _ended;

        void Awake()
        {
            if (!distance) distance = FindAnyObjectByType<DistanceTracker>(FindObjectsInactive.Exclude);
            if (!coins)    coins    = FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);
            if (!score)    score    = FindAnyObjectByType<RunScoreSystem>(FindObjectsInactive.Exclude);
            if (!speed)    speed    = FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);

            // Show start, hide game-over, pause time
            SetPanel(startPanel,  true);
            SetPanel(gameOverPanel, false);
            Time.timeScale = 0f;

            _started = false;
            _ended   = false;

            GameEvents.GameOver   += OnGameOver;
            GameEvents.GameStarted += OnGameStarted;
        }

        void OnDestroy()
        {
            GameEvents.GameOver   -= OnGameOver;
            GameEvents.GameStarted -= OnGameStarted;
        }

        // --- UI hooks ---
        public void OnPressStart()
        {
            if (_started) return;
            _started = true;

            // Reset and go
            coins?.ResetCoins(0);
            distance?.ResetDistance();
            speed?.ResetSpeed();
            score?.ResetRun();

            SetPanel(startPanel, false);
            Time.timeScale = 1f;

            distance?.StartRun();
            GameEvents.RaiseGameStarted();
        }

        public void OnPressRestart()
        {
            // Restart current scene (use SceneLoader if you prefer)
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        // --- Event handlers ---
        void OnGameOver()
        {
            if (_ended) return;
            _ended = true;

            // Stop progression, show menu
            distance?.StopRun();
            score?.CommitIfBest();

            SetPanel(gameOverPanel, true);
            Time.timeScale = 0f;
        }

        void OnGameStarted() { /* hook if you want SFX/UI, optional */ }

        static void SetPanel(GameObject go, bool on)
        {
            if (go && go.activeSelf != on) go.SetActive(on);
        }
    }
}