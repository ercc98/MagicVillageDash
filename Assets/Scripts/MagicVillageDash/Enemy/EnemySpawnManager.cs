// Assets/Scripts/MagicVillageDash/Enemies/EnemySpawnManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicVillageDash.Enemy;             // EnemyController, EnemyLifecycle
using MagicVillageDash.Runner;
using System;            // LaneRunner (implements ILaneMover)

namespace MagicVillageDash.Enemies
{
    public sealed class EnemySpawnManager : MonoBehaviour
    {
        [Header("Factory & Parent")]
        [SerializeField] private EnemyFactory enemyFactory;


        [Header("Lanes")]
        [SerializeField] private int laneCount = 3;
        [SerializeField] private int initialLane = 0;
        [SerializeField] private float currentLane = 0;


        [Header("Respawn")]
        [SerializeField] private float respawnDelay = 1.5f;


        void Awake()
        {
            enemyFactory = enemyFactory ? enemyFactory : FindAnyObjectByType<EnemyFactory>(FindObjectsInactive.Exclude);
        }

        void Start()
        {
            SpawnOne(initialLane);            
        }



        public EnemyController SpawnOne(int laneIndex)
        {
            EnemyController spawnedEnemy = enemyFactory.Spawn(laneIndex);
            spawnedEnemy.Ondied += HandleOndied;
            return spawnedEnemy;
        }

        void HandleOndied(EnemyController enemy)
        {
            enemy.Ondied -= HandleOndied;
            StartCoroutine(RespawnEnemyAfterDelay(respawnDelay));

        }

        IEnumerator RespawnEnemyAfterDelay(float respawnDelay)
        {
            yield return new WaitForSeconds(respawnDelay);
            SpawnOne(initialLane);
        }
    }
}
