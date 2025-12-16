using System.Collections;
using UnityEngine;
using MagicVillageDash.Enemy;
using ErccDev.Foundation.Core.Gameplay;
using System;

namespace MagicVillageDash.Enemies
{
    public sealed class EnemySpawnManager : MonoBehaviour, IEnemySpawner
    {
        [Header("Factory & Parent")]
        [SerializeField] private MonoBehaviour enemyFactoryProvider;

        [Header("Lanes")]
        [SerializeField] private int initialLane = 0;

        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 1.5f;


        IEnemyFactory enemyFactory;
        private Coroutine respawnRoutine;
        public event Action<EnemyController> OnSpawned;

        void Awake()
        {
            enemyFactory = enemyFactoryProvider as IEnemyFactory ?? FindAnyObjectByType<EnemyFactory>(FindObjectsInactive.Exclude);
        }

        void OnEnable()
        {
            GameEvents.GameOver += OnGameOver;
        }

        void OnDisable()
        {
            GameEvents.GameOver -= OnGameOver;
            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }
        }
        public void Spawn()
        {
            respawnRoutine = StartCoroutine(SpawnEnemyAfterDelay(respawnDelay));
        }
        
        IEnumerator SpawnEnemyAfterDelay(float respawnDelay)
        {
            yield return new WaitForSeconds(respawnDelay);
            OnSpawned?.Invoke(Spawn(initialLane));
        }

        public EnemyController Spawn(int laneIndex)
        {
            EnemyController spawnedEnemy = enemyFactory.Spawn(laneIndex);
            return spawnedEnemy;
        }

        private void OnGameOver()
        {
            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }
        }

    }
}
