using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;
using Unity.Jobs;

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
    private Entity enemyEntityPrefab;
    private WaitForSeconds spawnIntervalYield;
    private int id = 1;

    public struct SimpleJob : IJob
    {
        //[ReadOnly]
        public EntityCommandBuffer.Concurrent ConcurrentCommands;

        public int id;
        public NativeArray<float3> randoms;
        public NativeArray<float> randomSpeeds;
        public Entity enemyEntityPrefab;
        public int spawnCount;
        public float spawnRadius;
        public float minSpeed;
        public float maxSpeed;
        public float3 playerPos;

        public void Execute()
        {
            Debug.Log("Hello parallel world!");
            for (int i = 0; i < spawnCount; i++)
            {
                //var enemy = ConcurrentCommands.CreateEntity(i, enemyEntityPrefab);
                var enemyPrefab = ConcurrentCommands.Instantiate(id, enemyEntityPrefab);
                //var enemy = entityManager.Instantiate(enemyEntityPrefab);
                ConcurrentCommands.AddComponent(id, enemyPrefab, new Translation { Value = randoms[i] });
                ConcurrentCommands.AddComponent(id, enemyPrefab, new MoveForward { speed = randomSpeeds[i] });
                //ConcurrentCommands.SetComponentData(enemy, new Translation { Value = randoms[i] });
                //entityManager.SetComponentData(enemy, new MoveForward { speed = randomSpeeds[i] });
            }
        }
    }

    public struct SimpleJobParallelFor : IJobParallelFor
    {
        //[ReadOnly]
        public EntityCommandBuffer.Concurrent ConcurrentCommands;

        public int id;
        public NativeArray<float3> randoms;
        public NativeArray<float> randomSpeeds;
        public Entity enemyEntityPrefab;
        public int spawnCount;
        public float spawnRadius;
        public float minSpeed;
        public float maxSpeed;
        public float3 playerPos;

        public void Execute(int index)
        {
            //var enemy = ConcurrentCommands.CreateEntity(i, enemyEntityPrefab);
            var enemyPrefab = ConcurrentCommands.Instantiate(id, enemyEntityPrefab);
            //var enemy = entityManager.Instantiate(enemyEntityPrefab);
            ConcurrentCommands.AddComponent(id, enemyPrefab, new Translation { Value = randoms[index] });
            ConcurrentCommands.AddComponent(id, enemyPrefab, new MoveForward { speed = randomSpeeds[index] });
            //ConcurrentCommands.SetComponentData(enemy, new Translation { Value = randoms[i] });
            //entityManager.SetComponentData(enemy, new MoveForward { speed = randomSpeeds[i] });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnIntervalYield = new WaitForSeconds(spawnInterval);

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);

        //entityManager.Instantiate(enemyEntityPrefab);
        //SpawnWave();


        //// 1
        //var archetype = entityManager.CreateArchetype(
        //    typeof(Translation),
        //    typeof(Rotation),
        //    typeof(RenderMesh),
        //    typeof(RenderBounds),
        //    typeof(LocalToWorld));

        //// 2
        //var entity = entityManager.CreateEntity(archetype);

        //// 3
        //entityManager.AddComponentData(entity, new Translation { Value = new float3(-3f, 0.5f, 5f) });

        //entityManager.AddComponentData(entity, new Rotation { Value = quaternion.EulerXYZ(new float3(0f, 45f, 0f)) });

        //entityManager.AddSharedComponentData(entity, new RenderMesh
        //{
        //    mesh = enemy.GetComponent<MeshFilter>().sharedMesh,
        //    material = enemy.GetComponent<MeshRenderer>().sharedMaterial
        //});

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnWave1();
            yield return spawnIntervalYield;
        }
    }

    void SpawnWave1()
    {
        var ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        var job = new SimpleJobParallelFor();
        job.ConcurrentCommands = ecbSystem.CreateCommandBuffer().ToConcurrent();
        job.enemyEntityPrefab = enemyEntityPrefab;
        job.spawnCount = spawnCount;
        job.maxSpeed = maxSpeed;
        job.minSpeed = minSpeed;
        job.spawnRadius = spawnRadius;
        job.playerPos = GameManager.GetPlayerPosition();
        job.id = id++;

        job.randoms = new NativeArray<float3>(spawnCount, Allocator.TempJob);
        job.randomSpeeds = new NativeArray<float>(spawnCount, Allocator.TempJob);
        for (int i = 0; i < spawnCount; i++)
        {
            job.randoms[i] = RandomPointOnCircle(spawnRadius);
            job.randomSpeeds[i] = UnityEngine.Random.Range(minSpeed, maxSpeed);
        }

        var h = job.Schedule(spawnCount, 64);
        h.Complete();
        job.randomSpeeds.Dispose();
        job.randoms.Dispose();
    }

    void SpawnWave()
    {
        //NativeArray<Entity> enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);
        for (int i = 0; i < spawnCount; i++)
        {
            //enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);
            var enemy = entityManager.Instantiate(enemyEntityPrefab);
            entityManager.SetComponentData(enemy, new Translation { Value = RandomPointOnCircle(spawnRadius) });
            entityManager.SetComponentData(enemy, new MoveForward { speed = UnityEngine.Random.Range(minSpeed, maxSpeed) });
        }
        //enemyArray.Dispose();
        spawnCount += difficultyBonus;
    }

    float3 RandomPointOnCircle(float spawnRaduis)
    {
        var pos = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        return new float3(pos.x, 0, pos.y) + (float3)GameManager.GetPlayerPosition();
    }
}
