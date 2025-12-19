using System;
using System.Collections;
using ErccDev.Foundation.Core.Gameplay;
using MagicVillageDash.Character;
using MagicVillageDash.Runner;
using MagicVillageDash.World;
using UnityEngine;

namespace MagicVillageDash.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private EnemySensors nearSensor;
        [SerializeField] private EnemySensors midSensor;
        [SerializeField] private EnemySensors farSensor;
        [SerializeField] private MonoBehaviour characterControllerProvider;
        [SerializeField] protected MonoBehaviour gameSpeedProvider;
        [SerializeField] private GameObject Arrows;
        IMovementController movementController;
        ILaneMover laneMovement;
        IGameSpeedController gameSpeedController;

        [Header("Decision")]
        [SerializeField] private float startDelay = 2.0f;
        [SerializeField] private float adviceTime = 0.20f;
        [SerializeField] private float changeNearSensorPorcentaje = 0.3f;
        [SerializeField] private float changeFarSensorPorcentaje = 0.7f;
        [Tooltip("How often to think (seconds). 0.15 - 0.30 is a good range.")]
        [SerializeField] private float thinkInterval = 0.2f;

        [Tooltip("Minimum time between lane changes (anti-jitter).")]
        [SerializeField] private float moveCooldown = 0.25f;
        [SerializeField] private float timeWithoutCoins = 1.0f;

        private float nextThinkTime;
        private float nextMoveAllowedTime;
        private float nextMoveNoCoinsTime;
        private Action TurnMethod;

        void Awake()
        {
            movementController = characterControllerProvider as IMovementController ?? GetComponent<IMovementController>();
            laneMovement = characterControllerProvider as ILaneMover ?? GetComponent<ILaneMover>();
            gameSpeedController = gameSpeedProvider as IGameSpeedController ?? FindAnyObjectByType<GameSpeedController>(FindObjectsInactive.Exclude);
            if (movementController == null) Debug.LogError("EnemyAI: Missing IMovementController.", this);

            nextThinkTime       = Time.time + thinkInterval     +   startDelay;
            nextMoveAllowedTime = Time.time + moveCooldown      +   startDelay;
            nextMoveNoCoinsTime = Time.time + timeWithoutCoins  +   startDelay;
            Arrows.SetActive(false);
        }

        void OnEnable()
        {
            nearSensor.OnTriggerHit += OnTriggetHit;
            GameEvents.GameOver += OnGameOver;

            if (gameSpeedController.CurrentSpeed < gameSpeedController.MaxSpeed * changeNearSensorPorcentaje)
            {
                //Debug.Log("Near");
                nearSensor.gameObject.SetActive(true);
                farSensor.gameObject.SetActive(false);
                midSensor.gameObject.SetActive(false);

                nearSensor.OnTriggerHit += OnTriggetHit;
            }
            else if (gameSpeedController.CurrentSpeed > gameSpeedController.MaxSpeed * changeFarSensorPorcentaje)
            {
                //Debug.Log("farSensor");
                farSensor.gameObject.SetActive(true);
                nearSensor.gameObject.SetActive(false);
                midSensor.gameObject.SetActive(false);

                farSensor.OnTriggerHit += OnTriggetHit;
            }
            else
            {
                //Debug.Log("midSensor");
                midSensor.gameObject.SetActive(true);
                farSensor.gameObject.SetActive(false);
                nearSensor.gameObject.SetActive(false);
                
                midSensor.OnTriggerHit += OnTriggetHit;

            }
        }

        void OnDisable()
        {
            if (nearSensor != null) nearSensor.OnTriggerHit -= OnTriggetHit;
            if (farSensor != null) farSensor.OnTriggerHit -= OnTriggetHit;
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
            //FindCoins();
        }

        private void OnTriggetHit(string tag)
        {
            switch (tag)
            {
                case "Coin": Stay(); break;
                case "Hazard": Dodge(); break;
            }
        }
        private void FindCoins()
        {
            nextMoveNoCoinsTime = Time.time + timeWithoutCoins;
            Dodge();
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
            TurnMethod = movementController.TurnLeft;
            Arrows.transform.localScale = new Vector3(-1, Arrows.transform.localScale.y, Arrows.transform.localScale.z);           
            StartCoroutine(WaitForTurned());
        }

        private void TryMoveRight()
        {
            if (Time.time < nextMoveAllowedTime) return;
            TurnMethod = movementController.TurnRight;
            Arrows.transform.localScale = new Vector3(1, Arrows.transform.localScale.y, Arrows.transform.localScale.z);
            StartCoroutine(WaitForTurned());
        }
        
        IEnumerator WaitForTurned()
        {            
            Arrows.SetActive(true);
            yield return new WaitForSeconds(adviceTime);
            TurnMethod();
            nextMoveAllowedTime = Time.time + moveCooldown;
            yield return new WaitForSeconds(0.1f);
            Arrows.SetActive(false);
        }
    }
}
