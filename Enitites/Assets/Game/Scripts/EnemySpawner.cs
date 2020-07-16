using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Jobs;

namespace Game
{
    // https://www.raywenderlich.com/7630142-entity-component-system-for-unity-getting-started
    public class EnemySpawner : MonoBehaviour
    {
        EntityManager entityManager;

        [Header("Spawner")]

        [SerializeField]
        private int spawnCount = 30;

        [SerializeField]
        private float spawnInterval = 3f;

        [SerializeField]
        private int difficultyBonus = 5;

        [SerializeField]
        private float spawnRadius = 20f;

        [SerializeField]
        private float minSpeed = 4f;
        [SerializeField]
        private float maxSpeed = 10f;

        [Header("Enemy")]

        [SerializeField]
        private GameObject[] enemyPrefabs;
        private WaitForSeconds spawnIntervalYield;

        // Start is called before the first frame update
        void Start()
        {
            spawnIntervalYield = new WaitForSeconds(spawnInterval);

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var spawnSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SpawnSystem>();
            spawnSystem.spawnCount = spawnCount;
            spawnSystem.spawnInterval = spawnInterval;
            spawnSystem.minSpeed = minSpeed;
            spawnSystem.maxSpeed = maxSpeed;
            spawnSystem.spawnRadius = spawnRadius;
            spawnSystem.enemyPrefabs = new Entity[enemyPrefabs.Length];

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefabs[i], settings);
                spawnSystem.enemyPrefabs[i] = entity;
            }
        }
    }
}