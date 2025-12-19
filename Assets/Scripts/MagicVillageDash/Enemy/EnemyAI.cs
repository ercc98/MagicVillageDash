using System;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Character;
using MagicVillageDash.Runner;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private EnemySensors sensors;
        [SerializeField] private MonoBehaviour characterControllerProvider;
        private IMovementController movementController;
        private ILaneMover laneMovement;

        [Header("Decision")]
        [SerializeField] private float startDelay = 2.0f;
        [Tooltip("How often to think (seconds). 0.15 - 0.30 is a good range.")]
        [SerializeField] private float thinkInterval = 0.2f;

        [Tooltip("Minimum time between lane changes (anti-jitter).")]
        [SerializeField] private float moveCooldown = 0.25f;
        [SerializeField] private float timeWithoutCoins = 1.0f;

        private float nextThinkTime;
        private float nextMoveAllowedTime;
        private float nextMoveNoCoinsTime;

        void Awake()
        {
            if (!sensors) sensors = GetComponent<EnemySensors>();
            movementController = characterControllerProvider as IMovementController ?? GetComponent<IMovementController>();
            laneMovement = characterControllerProvider as ILaneMover ?? GetComponent<ILaneMover>();
            if (!sensors) Debug.LogError("EnemyAI: Missing EnemySensors.", this);
            if (movementController == null) Debug.LogError("EnemyAI: Missing IMovementController.", this);
            
            nextThinkTime       = Time.time + thinkInterval     +   startDelay;
            nextMoveAllowedTime = Time.time + moveCooldown      +   startDelay;
            nextMoveNoCoinsTime = Time.time + timeWithoutCoins  +   startDelay;
        }

        void OnEnable()
        {
            sensors.TriggerHit += OnTriggetHit;
            GameEvents.GameOver += OnGameOver;
        }

        void OnDisable()
        {
            sensors.TriggerHit -= OnTriggetHit;
            GameEvents.GameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            gameObject.SetActive(false);
        }
        void Update()
        {
            if (Time.time < nextThinkTime) return;
            nextThinkTime = Time.time + thinkInterval;
            if (Time.time < nextMoveNoCoinsTime) return;
            Dodge();
        }

        private void OnTriggetHit(string tag)
        {
            switch (tag)
            {
                case "Coin": Stay(); break;
                case "Hazard": Dodge(); break;
            }
        }

        private void Stay()
        {
            nextMoveNoCoinsTime = Time.time + timeWithoutCoins;
        }

        private void Dodge()
        {
            if (laneMovement.CurrentLane < 2)
                TryMoveRight();
            else
                TryMoveLeft();
        }

        private void TryMoveLeft()
        {
            if (Time.time < nextMoveAllowedTime) return;
            movementController.TurnLeft();
            nextMoveAllowedTime = Time.time + moveCooldown;
        }

        private void TryMoveRight()
        {
            if (Time.time < nextMoveAllowedTime) return;
            movementController.TurnRight();
            nextMoveAllowedTime = Time.time + moveCooldown;
        }
    }
}
