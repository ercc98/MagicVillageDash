using UnityEngine;
using UnityEngine.SceneManagement;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Score;
using MagicVillageDash.World;

namespace MagicVillageDash.UI
{
    public sealed class SimpleGameMenus : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameOverPanel;

        bool _started;
        bool _ended;

        void Awake()
        {

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
            
            SetPanel(startPanel, false);

            GameEvents.RaiseGameStarted();
        }


        // --- Event handlers ---
        void OnGameOver()
        {
            if (_ended) return;
            _ended = true;

            SetPanel(gameOverPanel, true);
            
        }

        void OnGameStarted() { /* hook if you want SFX/UI, optional */ }

        static void SetPanel(GameObject go, bool on)
        {
            if (go && go.activeSelf != on) go.SetActive(on);
        }
    }
}