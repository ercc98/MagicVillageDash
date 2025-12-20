using UnityEngine;
using UnityEngine.SceneManagement;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Score;
using TMPro;
using System.Collections;

namespace MagicVillageDash.UI
{
    public sealed class SimpleGameMenus : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private CoinCounter coinCounterProvider;
        [SerializeField] private string format = "{0}";
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] private DistanceTracker distanceCounterProvider;
        [SerializeField] private string distanceFormat = "{0} m"; // e.g., "123 m" 
        bool _started;
        bool _ended;
        ICoinCounter coinCounter;

        void Awake()
        {
            coinCounter = coinCounterProvider as ICoinCounter ?? FindAnyObjectByType<CoinCounter>(FindObjectsInactive.Exclude);

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
            Time.timeScale = 1f;

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
            coinText.SetText(format, coinCounter.Coins);
            distanceText.SetText(distanceFormat, distanceCounterProvider.CurrentDistance);
            StartCoroutine(WaitToGameOverPanel());
            
        }

        IEnumerator WaitToGameOverPanel()
        {
            yield return new WaitForSeconds(2f);
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