using System.Collections;
using UnityEngine;
using MagicVillageDash.Enemy;
using ErccDev.Foundation.Core.Gameplay;
using System;
using ErccDev.Foundation.Core.Factories;
using MagicVillageDash.Audio;
using ErccDev.Foundation.Audio;

namespace MagicVillageDash.Enemies
{
    public sealed class EnemySpawnManager : MonoBehaviour, IEnemySpawner
    {
        [SerializeField] private static WaitForSeconds _waitForSeconds1 = new(1f);
        [Header("Factory & Parent")]
        [SerializeField] private MonoBehaviour enemyFactoryProvider;

        [Header("Lanes")]
        [SerializeField] private int initialLane = 0;
        [SerializeField] private float laneWidth = 2.2f;

        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 1.5f;
        [SerializeField] private ParticleSystem spawnAreaParticleSystem;


        IFactory<EnemyController> enemyFactory;
        private Coroutine respawnRoutine;
        public event Action<EnemyController> OnSpawned;
        public event Action<int> OnStartSpawn;

        void Awake()
        {
            enemyFactory = enemyFactoryProvider as IFactory<EnemyController> ?? FindAnyObjectByType<EnemyFactory>(FindObjectsInactive.Exclude);
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
            OnStartSpawn?.Invoke(initialLane);
            float positionX = laneWidth * (initialLane - 1);
            spawnAreaParticleSystem.transform.position = new Vector3(positionX, 0.05f, 0);
            spawnAreaParticleSystem.Play();
            AudioManager.Instance?.Play("SpawnAreaEnemy", SoundCategory.SFX);
            yield return _waitForSeconds1;
            OnSpawned?.Invoke(Spawn(positionX));
        }

        public EnemyController Spawn(float positionX)
        {
            EnemyController spawnedEnemy = enemyFactory.Spawn(new Vector3(positionX, 1, 0), Quaternion.identity);
            AudioManager.Instance?.Play("SpawnEnemy", SoundCategory.SFX);
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
