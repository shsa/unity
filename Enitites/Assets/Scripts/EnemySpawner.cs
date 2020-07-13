using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;

// https://www.raywenderlich.com/7630142-entity-component-system-for-unity-getting-started
public class EnemySpawner : MonoBehaviour
{
    EntityManager entityManager;

    [SerializeField]
    private int spawnCount;

    [SerializeField]
    private int difficultyBonus = 0;

    [SerializeField]
    private int spawnRadius = 20;

    [SerializeField]
    private int minSpeed = 5;
    [SerializeField]
    private int maxSpeed = 10;

    [SerializeField]
    private GameObject enemyPrefab = null;
    private Entity enemyEntityPrefab;


    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);

        enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);

        //entityManager.Instantiate(enemyEntityPrefab);
        SpawnWave();


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

    }

    void SpawnWave()
    {
        // 1
        NativeArray<Entity> enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);

        // 2
        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] = entityManager.Instantiate(enemyEntityPrefab);

            // 3
            entityManager.SetComponentData(enemyArray[i], new Translation { Value = RandomPointOnCircle(spawnRadius) });

            // 4
            entityManager.SetComponentData(enemyArray[i], new MoveForward { speed = UnityEngine.Random.Range(minSpeed, maxSpeed) });
        }

        // 5
        enemyArray.Dispose();

        // 6
        spawnCount += difficultyBonus;
    }

    float3 RandomPointOnCircle(int spawnRaduis)
    {
        var pos = UnityEngine.Random.insideUnitCircle * spawnRadius;
        return new float3(pos.x, 0, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
