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
        private GameObject enemyPrefab = null;
        private WaitForSeconds spawnIntervalYield;

        // Start is called before the first frame update
        void Start()
        {
            spawnIntervalYield = new WaitForSeconds(spawnInterval);

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

            var enemyPartEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);

            var enemyEntityPrefab = entityManager.CreateArchetype(
                typeof(EnemyTag),
                typeof(Lifetime),
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld));

            var spawnSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SpawnSystem>();
            spawnSystem.spawnCount = spawnCount;
            spawnSystem.spawnInterval = spawnInterval;
            spawnSystem.enemyPartPrefab = enemyPartEntityPrefab;
            spawnSystem.enemyPrefab = enemyEntityPrefab;
            spawnSystem.minSpeed = minSpeed;
            spawnSystem.maxSpeed = maxSpeed;
            spawnSystem.spawnRadius = spawnRadius;

            //entityManager.AddSharedComponentData(entity, new RenderMesh
            //{
            //    mesh = enemy.GetComponent<MeshFilter>().sharedMesh,
            //    material = enemy.GetComponent<MeshRenderer>().sharedMaterial
            //});
        }
    }
}